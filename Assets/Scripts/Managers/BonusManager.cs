using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameDevLib.Audio;
using GameDevLib.Enums;
using RollABall.Args;
using RollABall.Events;
using RollABall.Interactivity.Bonuses;
using RollABall.Interactivity.Effects;
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
        [SerializeField] private BonusManagerStats stats;
        [Header("Prefabs")]
        [SerializeField] private GameObject bonusPrefab;
        [Header("Events")]
        [SerializeField] private BonusEvent bonusEvent;
        [Header("Links")]
        [SerializeField, Tooltip("Points on playing field for placing bonuses")] 
        private Transform[] bonusPoints;
        #endregion
        
        #region Fields
        
        private List<BonusItem> _positiveBonuses;
        private int _requiredNumberPositiveBonuses;
        private List<BonusItem> _negativeBonuses;
        private int _requiredNumberNegativeBonuses;

        private Dictionary<EffectFactoryKey, EffectFactory> _effectFactories;
        
        private readonly List<EffectFactoryKey> _positiveEffectKeys = new (3)
        {
            EffectFactoryKey.GpUp,
            EffectFactoryKey.HpUp,
            EffectFactoryKey.SpeedUp
        };
            
        private readonly List<EffectFactoryKey> _negativeEffectKeys = new (3)
        {
            EffectFactoryKey.GpDown,
            EffectFactoryKey.HpDown,
            EffectFactoryKey.SpeedDown
        };

        private AudioIsPlaying _audioIsPlaying;
        
        #endregion
        
        #region Properties 
        
        private BonusItem this[EffectType effectType, int i, bool isOverwrite] 
        {   
            get => (effectType == EffectType.Buff) ?  _positiveBonuses[i] : _negativeBonuses[i];
            set
            {
                var collection = effectType == EffectType.Buff ? _positiveBonuses : _negativeBonuses;
                var lenght = collection.Count - 1;
                
                if (isOverwrite)
                {
                    collection[i] = value;
                }
                else
                {
                    while(i < lenght && collection[i] != null) i++;
                    if(i < lenght)
                    {
                        collection[i] = value;
                    } 
                }
            }
        } 
        
        #endregion

        #region MonoBehaviour methods

        private void Start()
        {
            var halfOff = Math.DivRem(bonusPoints.Length, 2, out var remainder);
            _requiredNumberPositiveBonuses = halfOff;
            _requiredNumberNegativeBonuses = halfOff + remainder;
            
            _positiveBonuses = new List<BonusItem>(_requiredNumberPositiveBonuses);
            _negativeBonuses = new List<BonusItem>(_requiredNumberNegativeBonuses);
            
            MakeFactories();

            _audioIsPlaying = GetComponent<AudioIsPlaying>();
            
            StartCoroutine(BonusСheckСoroutine());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        #endregion

        #region Functionality

        private void MakeFactories()
        {
            _effectFactories = new Dictionary<EffectFactoryKey, EffectFactory>()
            {
                { EffectFactoryKey.GpUp, new AddGamePointsEffectFactory(stats.GpEffectValue, stats.GpEffectDuration)},
                { EffectFactoryKey.GpDown, new LostGamePointsEffectFactory(stats.GpEffectValue, stats.GpEffectDuration)},
                { EffectFactoryKey.HpUp, new AddHitPointsEffectFactory(stats.HpEffectValue, stats.HpEffectDuration)},
                { EffectFactoryKey.HpDown, new LostHitPointsEffectFactory(stats.HpEffectValue, stats.HpEffectDuration)},
                { EffectFactoryKey.SpeedUp, new AddHitPointsEffectFactory(stats.SpeedEffectMultiplier, stats.SpeedEffectDuration)},
                { EffectFactoryKey.SpeedDown, new LostHitPointsEffectFactory(stats.SpeedEffectMultiplier, stats.SpeedEffectDuration)}
            };
        }

        private List<Transform> FindFreePoints()
        {
            return bonusPoints
                .Where(t => t.gameObject.transform.childCount == 0)
                .ToList();
        }
        
        private IEnumerator BonusСheckСoroutine()
        {
            while (true)
            {
                if (_requiredNumberPositiveBonuses != _positiveBonuses.Count)
                {
                    var numberOfAddition = _requiredNumberPositiveBonuses - _positiveBonuses.Count;
                    yield return StartCoroutine(BonusPlacingСoroutine(EffectType.Buff, numberOfAddition));
                }
  
                if (_negativeBonuses.Count != _requiredNumberNegativeBonuses)
                {
                    var numberOfAddition = _requiredNumberNegativeBonuses - _negativeBonuses.Count;
                    yield return StartCoroutine(BonusPlacingСoroutine(EffectType.Debuff, numberOfAddition));
                }

                yield return null;
            }
        }
        
        private IEnumerator BonusPlacingСoroutine(EffectType type, int count)
        {
            var freePoints = FindFreePoints();
            
            var counter = count;
            while (counter != 0 && freePoints.Count > 0)
            {
                var randomIndex = Random.Range(0, freePoints.Count - 1);
                var randomPoint = freePoints[randomIndex];

                IBonusable newBonus = default;
                GameObject newBonusObject = default;
                
                newBonusObject = Instantiate(bonusPrefab, randomPoint.position, randomPoint.rotation);
                
                switch (type)
                {
                    case EffectType.Buff:
                        newBonus = newBonusObject.AddComponent<PositiveBonus>();
                        InitBonus(type, ref newBonus, randomPoint);
                        // HACK: Использую индексатор, хотя в данном кейсе это наверное не нужно
                        //this[EffectType.Buff, 0, false] = new BonusItem(randomPoint, newBonus); 
                        _positiveBonuses.Add(new BonusItem(randomPoint, newBonus));
                        break;
                    case EffectType.Debuff:
                    
                        newBonus = newBonusObject.AddComponent<NegativeBonus>();
                        InitBonus(type, ref newBonus, randomPoint); 
                        // HACK: Использую индексатор, хотя в данном кейсе это наверное не нужно
                        // this[EffectType.Debuff, 0, false] = new BonusItem(randomPoint, newBonus);
                        _negativeBonuses.Add(new BonusItem(randomPoint, newBonus));
                        break;
                }
                
                if (newBonus is not null)
                { 
                    newBonus.InteractiveNotify += OnGettingBonusNotify;
                    newBonusObject.tag = GameData.BonusTag;
                    newBonusObject.transform.parent = randomPoint;
                }

                freePoints.Remove(randomPoint);
                --counter;
                
                yield return new WaitForSeconds(stats.DelayAppearance);
                yield return null;
            }
        }

        /// <summary>
        /// Finds a random EffectFactoryKey according to EffectType, a factory and generates an effect.
        /// </summary>
        /// <param name="effectType">Type of effect to generate.</param>
        /// <returns>Random effect.</returns>
        private IEffectable GetRandomEffectByType(EffectType effectType)
        {
            var key = effectType switch
            {
                EffectType.Buff => _positiveEffectKeys[Random.Range(0, _positiveEffectKeys.Count - 1)],
                EffectType.Debuff => _negativeEffectKeys[Random.Range(0, _negativeEffectKeys.Count - 1)],
                _ => default
            };

            var factory = _effectFactories[key];
            return factory.GetEffect();
        }

        /// <summary>
        /// Specifies type of bonus by effect.
        /// </summary>
        /// <param name="effect">"Effect to specifies type of bonus.</param>
        /// <returns>Bonus type.</returns>
        private static BonusType GetBonusTypeByEffect(IEffectable effect)
        {
            BonusType bonusType = default;
            
            switch (effect.EffectTarget)
            {
                case EffectTargetType.GamePoints:
                    bonusType = effect.Type == EffectType.Buff ? BonusType.Gift : BonusType.Theft;
                    break;
                case EffectTargetType.HitPoints:
                    bonusType = effect.Type == EffectType.Buff ? BonusType.Booster : BonusType.Wound;
                    break;
                case EffectTargetType.UnitSpeed:
                    bonusType = effect.Type == EffectType.Buff ? BonusType.Booster : BonusType.TempSlowdown;
                    break;
            }

            return bonusType;
        }
        
        private void InitBonus(EffectType effectType, ref IBonusable bonus, Transform point)
        {
            var effect = GetRandomEffectByType(effectType);
            var bonusType = GetBonusTypeByEffect(effect);
            
            bonus.Init(bonusType, effect, point);
        }
        
        private void OnGettingBonusNotify(IBonusable bonus, string tagElement)
        {
            if (!string.Equals(tagElement, GameData.PlayerTag)) return;
            
            bonusEvent.Notify(new BonusArgs(GameData.PlayerTag, bonus.Effect));

            switch (bonus.Effect.Type)
            {
                case EffectType.Buff:
                    _audioIsPlaying.PlaySound(SoundType.Positive);
                    break;
                case EffectType.Debuff:
                    _audioIsPlaying.PlaySound(SoundType.Negative);
                    break;
            }

            StartCoroutine(RemoveBonusCoroutine(bonus));
        }

        private IEnumerator RemoveBonusCoroutine(IBonusable bonus)
        {
            var isSoundPlayed = false;
            _audioIsPlaying.AudioTriggerNotify += (played) =>
            {
                isSoundPlayed = played;
            };
            
            List<BonusItem> collection = default;

            switch (bonus.Effect.Type)
            {
                case EffectType.Buff:
                    collection = _positiveBonuses;
                    break;
                case EffectType.Debuff:
                    collection = _negativeBonuses;
                    break;
            }

            var existingElement = collection!.First(el => el.Bonus == bonus);
            existingElement.Bonus.InteractiveNotify -= OnGettingBonusNotify;

            if (existingElement.Point.gameObject.transform.childCount <= 0) yield break;
            var bonusGameObject = existingElement.Point.gameObject.transform.GetChild(0);

            yield return new WaitWhile(() => isSoundPlayed);
   
            Destroy(bonusGameObject.gameObject);

            yield return new WaitForSeconds(stats.DelayAfterRemove);
            
            collection.Remove(existingElement);
            
            yield return null;
        }
        #endregion
    }
}