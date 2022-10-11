using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameDevLib.Audio;
using GameDevLib.Enums;
using GameDevLib.Interfaces;
using Newtonsoft.Json;
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

        #endregion
        
        #region Properties
        public List<BonusManagerStateItem> State { get; set; }

        #endregion
        
        #region Fields
        
        private AudioIsPlaying _audioIsPlaying;
        
        #endregion
        
        #region Nested types
        
        
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
        
        protected override void InitManager()
        {
            StopAllCoroutines();

            if (BonusRepository.Count > 0)
            {
                BonusRepository.UpdateAllWithAction(UnactivateAndUnsubscribeAction);
            }
            else
            {
                // Filling collection of bonuses
                var counter = 0;
                while (counter < bonusPoints.Length)
                {
                    var point = bonusPoints[counter];
                    
                    var buffBonusItem = CreateBonusAndObject(EffectType.Buff, point);
                    var debuffBonusItem = CreateBonusAndObject(EffectType.Debuff, point);
                    
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
        
        /// <summary>
        /// Creates a new bonus and a game object for it.
        /// </summary>
        /// <param name="effectType">Type of effect for bonus.</param>
        /// <param name="point">Bonus point.</param>
        /// <returns></returns>
        private BonusItem CreateBonusAndObject(EffectType effectType, Transform point)
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
            
            var effect = EffectRepository.FindOnceByFilter(RandomEffectByType).Value;
            
            // Create game object
            var o = Instantiate(bonusPrefab, point.position, point.rotation);
            o.tag = GameData.BonusTag;
            o.SetActive(false);
            
            // Add and init bonus 
            IBonusable bonus = o.AddComponent<Bonus>();
            bonus.Init(effect, point);

            // Set point as parent for new object
            o.transform.parent = point;
            
            return new BonusItem(bonus, o);
        }
        
        /// <summary>
        /// Unsubscribes from events of all objects.
        /// </summary>
        /// <param name="value">Value to update.</param>
        private void UnactivateAndUnsubscribeAction(KeyValuePair<Transform, BonusItem[]> value)
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
            
            BonusRepository.UpdateOnceWithAction(existingPair.Key, UnactivateAndUnsubscribeAction);
            
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
                from bonusPair in BonusRepository.FindAll()
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

            // TODO: Все переопределить !!! 
        }

        public override void Dispose()
        {
            base.Dispose();
            StopAllCoroutines();
        }

        #endregion
    }
}