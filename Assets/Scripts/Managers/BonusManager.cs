using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameDevLib.Audio;
using GameDevLib.Enums;
using GameDevLib.Interfaces;
using RollABall.Args;
using RollABall.Interactivity.Bonuses;
using RollABall.Stats;
using UnityEngine;
using Random = UnityEngine.Random;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    [RequireComponent(typeof(AudioIsPlaying))]
    public class BonusManager : BaseManager
    {
        #region Links
        
        [SerializeField] private EffectStats stats;
        [SerializeField] private EffectManager effectManager;
        [Header("Prefabs")]
        [SerializeField] private GameObject bonusPrefab;
        [Header("Links")]
        [SerializeField, Tooltip("Points on playing field for placing bonuses")] 
        private BonusPoint[] bonusPoints;

        #endregion
        
        #region Fields
        
        private List<IBonusable> _positiveBonuses;
        private int _requiredNumberPositiveBonuses;
        private List<IBonusable> _negativeBonuses;
        private int _requiredNumberNegativeBonuses;

        private AudioIsPlaying _audioIsPlaying;

        private Coroutine _clearBonusesByEffectTypeCoroutine;
        private Coroutine _bonusPlacingBuffСoroutine;
        private Coroutine _bonusPlacingDebuffСoroutine;
        private Coroutine _removeBonusCoroutine;
        private Coroutine _removeOneBonusCoroutine;
        
        #endregion
        
        #region Properties 
        
        private IBonusable this[EffectType effectType, int i] 
        {   
            get => (effectType == EffectType.Buff) ?  _positiveBonuses[i] : _negativeBonuses[i];
            set
            {
                var collection = effectType == EffectType.Buff ? _positiveBonuses : _negativeBonuses;
                collection[i] = value;
            }
        } 
        
        #endregion

        #region MonoBehaviour methods

        private void Awake()
        {
            _audioIsPlaying = GetComponent<AudioIsPlaying>();
        }

        private void Start()
        {
            InitManager();
        }
        
        #endregion

        #region Functionality
        
        private void InitManager()
        {   
            _clearBonusesByEffectTypeCoroutine = _bonusPlacingBuffСoroutine = _bonusPlacingDebuffСoroutine =_removeBonusCoroutine = null;
            StopAllCoroutines();

            _clearBonusesByEffectTypeCoroutine = StartCoroutine(ClearBonusCollectionCoroutine());

            MakeCollections();
            CheckNumberOfBonuses();
        }

        private IEnumerator ClearBonusCollectionCoroutine()
        {
            yield return StartCoroutine(ClearBonusesByEffectType(EffectType.Buff));
            Debug.Log($"BonusManager: Clear buff collection complete.");
            yield return StartCoroutine(ClearBonusesByEffectType(EffectType.Debuff));
            Debug.Log($"BonusManager: Clear de-buff collection complete.");
            _clearBonusesByEffectTypeCoroutine = null;
        }
        
        private IEnumerator ClearBonusesByEffectType(EffectType effectType)
        {
            var collection = effectType == EffectType.Buff ? 
                _positiveBonuses : 
                _negativeBonuses;
            
            if (collection is null || collection.Count == 0) yield break;

            var counter = collection.Count;
            while (counter != 0)
            {
                var item = collection[counter - 1];
                yield return StartCoroutine(RemoveBonus(item, collection));
                --counter;
            }

            Debug.Assert(collection.Count == 0);

            yield return null;
        }

        /// <summary>
        /// Creates collections for storing bonuses on game board.
        /// </summary>
        private void MakeCollections()
        {
            var halfOff = Math.DivRem(bonusPoints.Length, 2, out var remainder);
            _requiredNumberPositiveBonuses = halfOff + remainder;
            _requiredNumberNegativeBonuses = halfOff;

            _positiveBonuses = new List<IBonusable>(_requiredNumberPositiveBonuses);
            _negativeBonuses = new List<IBonusable>(_requiredNumberNegativeBonuses);
        }
        
        /// <summary>
        /// Checks whether required number of bonuses is actually placed on playing field.
        /// </summary>
        private void CheckNumberOfBonuses()
        {
            var numberBuffsAdded = _requiredNumberPositiveBonuses - _positiveBonuses.Count;
            var numberDebuffsAdded = _requiredNumberNegativeBonuses - _negativeBonuses.Count;
            
            if (numberBuffsAdded > 0)
            {
                _bonusPlacingBuffСoroutine = StartCoroutine(BonusPlacingСoroutine(EffectType.Buff, numberBuffsAdded));
            }
            
            if (numberDebuffsAdded > 0)
            {
                _bonusPlacingDebuffСoroutine = StartCoroutine(BonusPlacingСoroutine(EffectType.Debuff, numberDebuffsAdded));
            }
        }

        /// <summary>
        /// Reserves and returns free BonusPoints on playing field.
        /// </summary>
        /// <param name="count">Required number of free points.</param>
        /// <returns>List with reserved free points.</returns>
        private List<BonusPoint> GetFreeBonusPoints(int count)
        {
            var selectedPoints = bonusPoints
                .Where(p => p.IsReserved == false)
                .Take(count)
                .Select(p => { p.Reserve(); return p; })
                .ToList();

            return selectedPoints;
        }

        /// <summary>
        /// Places new bonuses on playing field.
        /// </summary>
        /// <param name="effectType">Type of effect for group of spreadable bonuses.</param>
        /// <param name="count">Number of placed bonuses.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private IEnumerator BonusPlacingСoroutine(EffectType effectType, int count)
        {
            yield return new WaitUntil(() => effectType == EffectType.Buff ? 
                _bonusPlacingDebuffСoroutine == null :
                _bonusPlacingBuffСoroutine == null);

            yield return new WaitUntil(() => _removeBonusCoroutine == null);

            var freePoints = GetFreeBonusPoints(count);
            
            var counter = count;
            while (counter > 0 && freePoints.Count > 0)
            {
                var randomIndex = Random.Range(0, freePoints.Count - 1);
                var randomPoint = freePoints[randomIndex];
                
                var collection = effectType == EffectType.Buff ? _positiveBonuses : _negativeBonuses;
                IBonusable newBonus;
                
                var effect = effectManager.GetRandomEffectByType(effectType);
                var newBonusObject = Instantiate(bonusPrefab, randomPoint.Point.position, randomPoint.Point.rotation);
                newBonusObject.tag = GameData.BonusTag;
                newBonus = newBonusObject.AddComponent<Bonus>();
                newBonus.Init(effect, randomPoint);
                newBonus.InteractiveNotify += OnBonusNotify;

                collection.Add(newBonus);

                newBonusObject.transform.parent = randomPoint.Point;

                freePoints.Remove(randomPoint);

                --counter;
                
                yield return new WaitForSeconds(stats.DelayAppearance);
            }
            
            if (effectType == EffectType.Buff)
            {
                _bonusPlacingBuffСoroutine = null;
            }
            else
            {
                _bonusPlacingDebuffСoroutine = null;
            }
        }
        
        /// <summary>
        /// Called when a bonus collision occurs. 
        /// </summary>
        /// <param name="bonus">Bonus that came across.</param>
        /// <param name="tagElement">Tag of element that the bonus encountered.</param>
        private void OnBonusNotify(IBonusable bonus, string tagElement)
        {
            if (string.Equals(tagElement, GameData.PlayerTag))
            {
                effectManager.ApplyEffectOnPlayer(bonus.Effect);
            }
            
            _audioIsPlaying.PlaySound(bonus.Effect.Type == EffectType.Buff ? 
                SoundType.Positive : 
                SoundType.Negative);
            
            _removeBonusCoroutine = StartCoroutine(RemoveBonusCoroutine(bonus));
        }

        /// <summary>
        /// Removes a bonus from collection of placed bonuses and a game object from field. 
        /// </summary>
        /// <param name="bonus">Removal Bonus.</param>
        private IEnumerator RemoveBonusCoroutine(IBonusable bonus)
        {
            var collection = bonus.Effect.Type == EffectType.Buff ?_positiveBonuses : _negativeBonuses;
            var existingBonus = collection!.FirstOrDefault(el => el == bonus);
            
            if (existingBonus != null)
            {
                yield return StartCoroutine(RemoveBonus(existingBonus, collection, true));
            }
            
            _removeBonusCoroutine = null;
            CheckNumberOfBonuses();

            yield return null;
        }

        private IEnumerator RemoveBonus(IBonusable bonusable, ICollection<IBonusable> collection, bool delay = false)
        {
            // Unsubscribing from a bonus event
            bonusable.InteractiveNotify -= OnBonusNotify;
            
            // Clear bonus point
            var bonusPoint = bonusPoints.First(p => p == bonusable.Point);
            bonusPoint.Clear();
            
            // Remove bonus game object
            var childrenCount = bonusable.GetChildrenCount();
            if (childrenCount > 0)
            {
                // Find and disable all children
                var gameObjects = new List<GameObject>();
                for (var i = 0; i < childrenCount; ++i)
                {
                    var obj = bonusable.Point.Point.transform.GetChild(i).gameObject;
                    obj.SetActive(false);
                    gameObjects.Add(obj);
                }
                
                //Wait until play audio
                if (delay)
                {
                    var isSoundPlayed = false;
                    _audioIsPlaying.AudioTriggerNotify += (played) =>
                    {
                        isSoundPlayed = played;
                    };
                
                    yield return new WaitUntil(() => isSoundPlayed);
                }

                foreach (var go in gameObjects)
                {
                    Destroy(go); 
                }
            }
            
            collection.Remove(bonusable);

            if (delay)
            {
                yield return new WaitForSeconds(stats.DelayAfterRemove);
            }
            else
            {
                yield return null;
            }
            
        }
        
        // Event handler for CurrentGameEvent
        public override void OnEventRaised(ISubject<CurrentGameArgs> subject, CurrentGameArgs args)
        {
            if (args.IsRestartGame)
            {
                InitManager();
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            StopAllCoroutines();
        }

        #endregion
    }
}