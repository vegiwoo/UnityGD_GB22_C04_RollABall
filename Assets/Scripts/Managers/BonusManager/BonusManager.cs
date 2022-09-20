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
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;
using static UnityEngine.Debug;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    [RequireComponent(typeof(AudioIsPlaying))]
    public class BonusManager : MonoBehaviour
    {
        #region Links
        [Header("Stats")]
        [SerializeField] private EffectStats stats;
        [Header("Prefabs")]
        [SerializeField] private GameObject bonusPrefab;
        [Header("Events")]
        [SerializeField] private BonusEvent bonusEvent;
        [Header("Links")]
        [SerializeField, Tooltip("Points on playing field for placing bonuses")] 
        private Transform[] bonusPoints;
        #endregion
        
        #region Fields
        
        // Placed bonuses on field
        private List<BonusItem> _positiveBonuses;
        private int _requiredNumberPositiveBonuses;
        private List<BonusItem> _negativeBonuses;
        private int _requiredNumberNegativeBonuses;

        // Stored effects by type
        private List<Effect> _buffs;
        private List<Effect> _debuffs;

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
            _requiredNumberPositiveBonuses = halfOff + remainder;
            _requiredNumberNegativeBonuses = halfOff;
            
            _positiveBonuses = new List<BonusItem>(_requiredNumberPositiveBonuses);
            _negativeBonuses = new List<BonusItem>(_requiredNumberNegativeBonuses);

            try
            {
                _buffs = stats.effects.Where(el => el.Type == EffectType.Buff).ToList();
                _debuffs = stats.effects.Where(el => el.Type == EffectType.Debuff).ToList();
            }
            catch (ArgumentNullException e)
            {
                LogError("Link to effect stats cannot be empty");
                EditorApplication.isPlaying = false;
            }

            _audioIsPlaying = GetComponent<AudioIsPlaying>();
            
            StartCoroutine(BonusСheckСoroutine());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        #endregion

        #region Functionality

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
                    newBonus.InteractiveNotify += OnBonusNotify;
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
            if (stats == null)
            {
                throw new ArgumentNullException(stats.effects.ToString());
            }
            
            return effectType switch
            {
                EffectType.Buff => _buffs[Random.Range(0, _buffs.Count - 1)],
                EffectType.Debuff => _debuffs[Random.Range(0, _debuffs.Count - 1)],
                _ => default
            };
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
        
        private void OnBonusNotify(IBonusable bonus, string tagElement)
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
            existingElement.Bonus.InteractiveNotify -= OnBonusNotify;

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