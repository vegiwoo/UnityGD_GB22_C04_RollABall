using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameDevLib.Audio;
using GameDevLib.Enums;
using GameDevLib.Interfaces;
using RollABall.Args;
using RollABall.Interactivity.Bonuses;
using RollABall.Stats;
using Unity.VisualScripting;
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
        private Transform[] bonusPoints;

        #endregion
        
        #region Fields
        
        private AudioIsPlaying _audioIsPlaying;

        #endregion
        
        #region Properties 
        // ... 
        #endregion

        // New solution 
        private Dictionary<Transform, (IBonusable bonus, GameObject bonusGO)[]> _bonusPool;

        // New solution 
        private (IBonusable bonus, GameObject bonusGameObject) CreateBonusObject(EffectType effectType, Transform point)
        {
            // Create effect 
            var effect = effectManager.GetRandomEffectByType(effectType);
            
            // Create game object
            var o = Instantiate(bonusPrefab, point.position, point.rotation);
            o.tag = GameData.BonusTag;
            o.SetActive(false);
            
            // Add and init bonus 
            IBonusable bonus = o.AddComponent<Bonus>();
            bonus.Init(effect, point);

            // Set parent for new object
            o.transform.parent = point;
            
            return (bonus, o);
        }
        
        private void InitManager()
        {
            StopAllCoroutines();
            
            if (_bonusPool is not null)
            {
                foreach (var (p, b) in _bonusPool)
                {
                    // Unsubscribing from collision events with a bonus
                    foreach (var item in b)
                    {
                        item.bonus.InteractiveNotify -= OnBonusNotify;
                    }
                    
                    // Deactivation of all game objects
                    var bonusGameObjectsInPointCount = p.childCount;
                    for (var i = 0; i < bonusGameObjectsInPointCount; ++i)
                    {
                        var bonusGameObject = p.GetChild(i);
                        bonusGameObject.gameObject.SetActive(false);
                    }
                }
            }
            else 
            {
                // Creating a dictionary for storing bonuses.
                _bonusPool = new Dictionary<Transform, (IBonusable bonus, GameObject bonusGO)[]>(bonusPoints.Length);

                var counter = 0;
                while (counter < bonusPoints.Length)
                {
                    var point = bonusPoints[counter];
                    
                    var buffBonusItem = CreateBonusObject(EffectType.Buff, point);
                    var debuffBonusItem = CreateBonusObject(EffectType.Debuff, point);
                    
                    _bonusPool.Add(point, new [] { buffBonusItem, debuffBonusItem });

                    ++counter;
                }
            }
            
            // Activating one random bonus at each point.
            foreach (var (p, items) in _bonusPool)
            {
                var randomIndex = Random.Range(0, items.Length);
                items[randomIndex].bonusGO.SetActive(true);
                items[randomIndex].bonus.InteractiveNotify += OnBonusNotify;
            }
        }

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
            
            StartCoroutine(ToggleBonusCoroutine(bonus));
        }

        private IEnumerator ToggleBonusCoroutine(IBonusable bonusable)
        {
            // Unsubscribing from collision events with a bonus
            bonusable.InteractiveNotify -= OnBonusNotify;
            
            var poolElement = 
                _bonusPool.First(el => el.Key == bonusable.Point);
            
            // Deactivation of all game objects in pool element
            var bonusGameObjectsInPointCount = poolElement.Key.childCount;
            for (var i = 0; i < bonusGameObjectsInPointCount; ++i)
            {
                var bonusGameObject = poolElement.Key.GetChild(i);
                bonusGameObject.gameObject.SetActive(false);
            }

            // Audio Completion Waiting
            var isSoundPlayed = false;
            _audioIsPlaying.AudioTriggerNotify += (played) => { isSoundPlayed = played; };

            yield return new WaitUntil(() => isSoundPlayed);
            
            yield return new WaitForSeconds(stats.DelayAfterRemove);
            
            // Find and activate new bonus in pool element
            var otherBonusesInPoolElement = poolElement.Value.First(b => b .bonus != bonusable);
            otherBonusesInPoolElement.bonusGO.SetActive(true);
            otherBonusesInPoolElement.bonus.InteractiveNotify += OnBonusNotify;
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