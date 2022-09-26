using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameDevLib.Audio;
using GameDevLib.Enums;
using RollABall.Interactivity.Bonuses;
using RollABall.Stats;
using UnityEngine;
using Random = UnityEngine.Random;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    [RequireComponent(typeof(AudioIsPlaying))]
    public class BonusManager : MonoBehaviour
    {
        #region Links
        [Header("Stats")]
        [SerializeField] private EffectStats stats;
        [SerializeField] private EffectManager effectManager;
        [Header("Prefabs")]
        [SerializeField] private GameObject bonusPrefab;
        [Header("Links")]
        [SerializeField, Tooltip("Points on playing field for placing bonuses")] 
        private BonusPoint[] bonusPoints;
        #endregion
        
        #region Fields
        
        // Placed bonuses on field
        private List<IBonusable> _positiveBonuses;
        private int _requiredNumberPositiveBonuses;
        private List<IBonusable> _negativeBonuses;
        private int _requiredNumberNegativeBonuses;

        private AudioIsPlaying _audioIsPlaying;

        private Coroutine _removeBonusCoroutine;
        
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
            MakeCollections();
            CheckNumberOfBonuses(EffectType.Buff);
            CheckNumberOfBonuses(EffectType.Debuff);
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        #endregion

        #region Functionality
        
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
        /// <param name="effectType">Effect type (buff or debuff).</param>
        private void CheckNumberOfBonuses(EffectType effectType)
        {
            var numberOfAddition = effectType == EffectType.Buff ? 
                _positiveBonuses.Capacity - _positiveBonuses.Count : 
                _negativeBonuses.Capacity - _negativeBonuses.Count;
            
            if (numberOfAddition > 0)
            {
                StartCoroutine(BonusPlacingСoroutine(effectType, numberOfAddition));
            }
        }

        /// <summary>
        /// Reserves and returns free BonusPoints on playing field.
        /// </summary>
        /// <param name="count">Required number of free points.</param>
        /// <returns>List with reserved free points.</returns>
        private List<BonusPoint> GetFreeBonusPoints(int count)
        {
            var points = bonusPoints
                .Where(p => p.IsReserved == false)
                .Take(count)
                .ToList();
            
            foreach (var point in points)
            {
                point.Reserve();
            }
            
            return points;
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
            yield return new WaitUntil(() => _removeBonusCoroutine == null);
            
            var freePoints = GetFreeBonusPoints(count);
            
            var counter = count;
            while (counter > 0 && freePoints.Count > 0)
            {
                var randomIndex = Random.Range(0, freePoints.Count - 1);
                var randomPoint = freePoints[randomIndex];
                
                var collection = effectType == EffectType.Buff ? _positiveBonuses : _negativeBonuses;
                IBonusable newBonus = default;
                
                var newBonusObject = Instantiate(bonusPrefab, randomPoint.Point.position, randomPoint.Point.rotation);
                newBonusObject.tag = GameData.BonusTag;
                newBonus = newBonusObject.AddComponent<Bonus>();
                
                var effect = effectManager.GetRandomEffectByType(effectType);

                newBonus.Init(effect, randomPoint);
                newBonus.InteractiveNotify += OnBonusNotify;

                collection.Add(newBonus);

                newBonusObject.transform.parent = randomPoint.Point;
                
                freePoints.Remove(randomPoint);
                --counter;

                yield return new WaitForSeconds(stats.DelayAppearance);
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
                var existingBonusGameObject = existingBonus.Point.Point.gameObject.transform.GetChild(0).gameObject;
                existingBonusGameObject.SetActive(false);
                
                existingBonus.InteractiveNotify -= OnBonusNotify;
                var isSoundPlayed = false;
                _audioIsPlaying.AudioTriggerNotify += (played) =>
                {
                    isSoundPlayed = played;
                };

                yield return new WaitUntil(() => isSoundPlayed);

                Destroy(existingBonusGameObject);
                existingBonus.Point.Clear();
                collection.Remove(existingBonus);
                    
                yield return new WaitForSeconds(stats.DelayAfterRemove);
                
                _removeBonusCoroutine = null;
                
                CheckNumberOfBonuses(bonus.Effect.Type);
            }
            else
            {
                yield return null;
            }

        }

        #endregion
    }
}