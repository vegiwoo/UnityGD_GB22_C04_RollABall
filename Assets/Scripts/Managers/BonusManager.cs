using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameDevLib.Audio;
using GameDevLib.Enums;
using GameDevLib.Interfaces;
using RollABall.Args;
using RollABall.Events;
using RollABall.Infrastructure.Memento;
using RollABall.Interactivity.Bonuses;
using RollABall.ScriptableObjects;
using RollABall.Stats;
using UnityEngine;
using static UnityEngine.Debug;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    [RequireComponent(typeof(AudioIsPlaying))]
    public class BonusManager : BaseManager, IMementoOrganizer<List<BonusManagerStateItem>>
    {
        #region Links
        
        [SerializeField] private EffectStats stats;
        [Header("Prefabs")]
        [SerializeField] private GameObject bonusPrefab;
        [Header("Links")]
        [SerializeField, Tooltip("Points on playing field for placing bonuses")] 
        private Transform[] bonusPoints;
        [field:SerializeField] public BonusManagerCaretaker Caretaker { get; set; }
        [field: SerializeField] private BonusRepository BonusRepository { get; set; }
        [field: SerializeField] private EffectRepository EffectRepository { get; set; }
        [field: SerializeField] private ApplyEffectEvent ApplyEffectEvent { get; set; }
        [field: SerializeField] private BonusManagerEvent BonusManagerEvent { get; set; }

        #endregion
        
        #region Properties
        public List<BonusManagerStateItem> State { get; set; }

        #endregion
        
        #region Fields
        private const int BonusInPoint = 2;
        private AudioIsPlaying _audioIsPlaying;
        
        #endregion

        #region MonoBehaviour methods

        private void Awake()
        {
            _audioIsPlaying = GetComponent<AudioIsPlaying>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            // Memento pattern - init caretaker for organizer.
            Caretaker.Init(this, "Bonuses", "BonusMemento");
        }

        #endregion
        
        #region Functionality
        
        protected override void InitManager(bool fromLoad = false)
        {
            StopAllCoroutines();

            if (fromLoad && State != null)
            {
                // Clear repo
                BonusRepository.UpdateAllWithAction(UnsubscribeAndDestroyAction);
                BonusRepository.RemoveAll();
                
                // Cleanup notify
                BonusManagerEvent.Notify(new BonusManagerArgs(null,null,null,true));
                
                // Make items from loading file
                const float pointTolerance = 0.05f;
                
                foreach (var bonusPoint in bonusPoints)
                {
                    var entities = State.Where(it =>
                    {
                        Vector3 position;
                        return Math.Abs(it.Point.PosX - (position = bonusPoint.position).x) < pointTolerance &&
                               Math.Abs(it.Point.PosZ - position.z) < pointTolerance;
                    }).ToList();

                    var pair = new KeyValuePair<Transform, BonusItem[]>(bonusPoint, new BonusItem[2]);

                    for (var i = 0; i < entities.Count(); i++)
                    {
                        var bonusItem = CreateBonusAndObject(null, entities[i].Effect, bonusPoint, entities[i].IsActive);
                        pair.Value[i] = bonusItem;
                    }

                    try
                    {
                        BonusRepository.Insert(pair.Key, pair.Value);
                    }
                    catch (Exception e)
                    {
                        LogException(e);
                    }
                }
            }
            else
            {
                // Restart game
                if (BonusRepository.Count > 0)
                {
                    BonusRepository.UpdateAllWithAction(UnsubscribeAndUnactivateAction);
                }
                else
                {
                    // New game 
                    var counter = 0;
                    while (counter < bonusPoints.Length)
                    {
                        var point = bonusPoints[counter];
                    
                        var buffBonusItem = CreateBonusAndObject(EffectType.Buff, null, point);
                        var debuffBonusItem = CreateBonusAndObject(EffectType.Debuff, null, point);
                    
                        try
                        {
                            BonusRepository.Insert(point,new [] { buffBonusItem, debuffBonusItem });
                        }
                        catch (Exception e)
                        {
                            LogException(e);
                        }

                        ++counter;
                    }
                }
                
                BonusRepository.UpdateAllWithAction(RandomActivateAndSubscribeAction);
            }
            
            // Placement notify
            BonusManagerEvent.Notify(new BonusManagerArgs(bonusPoints,null,null,false));
        }

        /// <summary>
        /// Creates a new bonus and a game object for it.
        /// </summary>
        /// <param name="effectType">Type of effect for bonus.</param>
        /// <param name="point">Bonus point.</param>
        /// <param name="isActive">Whether the item being created is active.</param>
        /// <param name="effect">Ready effect to assign to the bonus (optional).</param>
        /// <returns></returns>
        private BonusItem CreateBonusAndObject(EffectType? effectType, Effect? effect, Transform point, bool isActive = false)
        {
            IEffectable createdEffect;
            if (effect == null && effectType.HasValue)
            {
                // Create effect 
                KeyValuePair<Guid, IEffectable> RandomEffectByType(IDictionary<Guid, IEffectable> collection)
                {
                    var effectsByType = collection
                        .Where(el => el.Value.Type == effectType)
                        .ToList();
                
                    var randomIndex = systemRandom.Next(0, effectsByType.Count);
                
                    return effectsByType[randomIndex];
                }
            
                createdEffect = EffectRepository.FindOnceByFilter(RandomEffectByType).Value;
            }
            else
            {
                createdEffect = effect;
            }

            // Create game object
            var o = Instantiate(bonusPrefab, point.position, point.rotation);
            o.tag = GameData.BonusTag;
            o.SetActive(isActive);
            
            // Add and init bonus 
            IBonusable bonus = o.AddComponent<Bonus>();
            bonus.Init(createdEffect, point);

            if (isActive)
            {
                bonus.InteractiveNotify += OnBonusNotify;
            }

            // Set point as parent for new object
            o.transform.parent = point;
            
            return new BonusItem(bonus, o);
        }

        /// <summary>
        /// Unsubscribes from events and unactivate of all objects on point.
        /// </summary>
        /// <param name="value">Value to update.</param>
        private void UnsubscribeAndUnactivateAction(KeyValuePair<Transform, BonusItem[]> value)
        {
            // Unsubscribing from collision events with a bonus in pair
            foreach (var item in value.Value)
            {
                item.Bonus.InteractiveNotify -= OnBonusNotify;
            }

            // Deactivation of all game objects in pair
            var bonusGameObjectsInPointCount = value.Key.childCount;
            for (var i = 0; i < bonusGameObjectsInPointCount; ++i)
            {
                var bonusGameObject = value.Key.GetChild(i);
                bonusGameObject.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Unsubscribes from events and destroy all objects on point.
        /// </summary>
        /// <param name="value">Value to update.</param>
        private void UnsubscribeAndDestroyAction(KeyValuePair<Transform, BonusItem[]> value)
        {
            // Unsubscribing from collision events with a bonus in pair
            foreach (var item in value.Value)
            {
                item.Bonus.InteractiveNotify -= OnBonusNotify;
            }
            
            // Remove of all game objects in pair
            var bonusGameObjectsInPointCount = value.Key.childCount;
            for (var i = 0; i < bonusGameObjectsInPointCount; ++i)
            {
                var bonusGoTransform = value.Key.GetChild(i);
                if (bonusGoTransform != null && bonusGoTransform.gameObject != null)
                {
                    Destroy(bonusGoTransform.gameObject);
                }
            }
        }
        
        /// <summary>
        /// Randomly activates one of bonuses on point.
        /// </summary>
        /// <param name="value">Value to update.</param>
        private void RandomActivateAndSubscribeAction(KeyValuePair<Transform, BonusItem[]> value)
        {
            var element = value.Value[systemRandom.Next(0, value.Value.Length)];
            
            element.BonusGo.SetActive(true);
            element.Bonus.InteractiveNotify += OnBonusNotify;
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
                ApplyEffectEvent.Notify(bonus.Effect);
            }
            
            _audioIsPlaying.PlaySound(bonus.Effect.Type == EffectType.Buff ? 
                SoundType.Positive : 
                SoundType.Negative);
            
            StartCoroutine(ToggleBonusCoroutine(bonus));
        }

        private IEnumerator ToggleBonusCoroutine(IBonusable bonusable)
        {
            bool IsMatch(IEnumerable<BonusItem> items)
            {
                return items.Any(el => el.Bonus == bonusable);
            }

            var existingPair = BonusRepository.FindOnceByFilter(IsMatch);
            
            // Unsubscribe and deactivation bonus
            BonusRepository.UpdateOnceWithAction(existingPair.Key, UnsubscribeAndUnactivateAction);

            // Deactivation bonus point notify
            BonusManagerEvent.Notify(new BonusManagerArgs(null,existingPair.Key,null,false));

            // Audio Completion Waiting
            var isSoundPlayed = false;
            _audioIsPlaying.AudioTriggerNotify += (played) => { isSoundPlayed = played; };
            
            // Delay 
            yield return new WaitUntil(() => isSoundPlayed); 
            yield return new WaitForSeconds(stats.DelayAfterRemove);
            
            // Toggle bonus in pair
            void ToggleAction(KeyValuePair<Transform, BonusItem[]> pair)
            {
                if (pair.Value.Length != 2) return;

                var otherBonus = existingPair.Value.First(el => el.Bonus != bonusable);
                otherBonus.Bonus.InteractiveNotify += OnBonusNotify;
                otherBonus.BonusGo.SetActive(true);
            }

            BonusRepository.UpdateOnceWithAction(existingPair.Key, ToggleAction);
            
            // Activation point notify
            BonusManagerEvent.Notify(new BonusManagerArgs(null,null,existingPair.Key,false));
        }

        // Event handler for CurrentGameEvent

        public override void OnEventRaised(ISubject<CurrentGameArgs> subject, CurrentGameArgs args)
        {
            if (args.IsRestartGame)
            {
                InitManager();
            }

            if (args.IsSaveGame)
            {
                try
                {
                    Caretaker.Save();
                }
                catch (Exception e)
                {
                    LogException(e);
                }
            }

            if (args.IsLoadGame)
            {
                try
                {
                    Caretaker.Load();
                }
                catch (Exception e)
                {
                    LogException(e);
                }
            }
        }   

        // Memento pattern methods
        public List<BonusManagerStateItem> MakeState()
        {
            var items =
                from bonusPair in BonusRepository.FindAllValues()
                from bonus in bonusPair
                select new BonusManagerStateItem(bonus.Bonus.Point.position, bonus.Bonus.Effect as Effect,
                    bonus.BonusGo.activeInHierarchy);
            
            return items.ToList();;
        }

        public IMemento<List<BonusManagerStateItem>> Save()
        {
            State = MakeState();
            return new BonusManagerMemento(State);
        }

        public void Load(IMemento<List<BonusManagerStateItem>> memento)
        {
            if (memento is not BonusManagerMemento)
            {
                throw new Exception("Unknown memento class " + memento.ToString());
            }
            
            State = memento.State;
            InitManager(true);
        }

        public override void Dispose()
        {
            base.Dispose();
            StopAllCoroutines();
        }

        #endregion
    }
}