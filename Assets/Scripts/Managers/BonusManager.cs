using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public class BonusManager : MonoBehaviour
    {
        class BonusItem : IEquatable<BonusItem>
        {
            public Transform Point { get; }
            public IBonusRepresentable Bonus { get; }

            public BonusItem(Transform point, IBonusRepresentable bonus)
            {
                Point = point;
                Bonus = bonus;
            }
            
            public bool Equals(BonusItem? other)  
            {        
                if (ReferenceEquals(null, other)) return false;  
                if (ReferenceEquals(this, other)) return true;  
                return Point == other.Point && Bonus == other.Bonus;  
            }  
        }
        
        #region Links
        [Header("Stats")]
        [SerializeField] private BonusManagerStats stats;
        [Header("Prefabs")]
        [SerializeField] private GameObject positiveBonusPrefab;
        [SerializeField] private GameObject negativeBonusPrefab;
        [Header("Events")]
        [SerializeField] private BonusEvent bonusEvent;
        [Header("Links")]
        [SerializeField, Tooltip("Points on playing field for placing bonuses")] 
        private Transform[] bonusPoints;
        #endregion
        
        // TODO: Бонусы в одном классе с 2 мя списками (+ и -), индексаторы для этих списков
        
        #region Constant and variables
        private List<BonusItem> _positiveBonuses;
        private int _requiredNumberPositiveBonuses;
        private List<BonusItem> _negativeBonuses;
        private int _requiredNumberNegativeBonuses;
        private List<Effect> effects;
        #endregion

        #region MonoBehaviour methods

        private void Start()
        {
            var halfOff = Math.DivRem(bonusPoints.Length, 2, out var remainder);
            _requiredNumberPositiveBonuses = halfOff;
            _requiredNumberNegativeBonuses = halfOff + remainder;
            
            _positiveBonuses = new List<BonusItem>(_requiredNumberPositiveBonuses);
            _negativeBonuses = new List<BonusItem>(_requiredNumberNegativeBonuses);
            
            MakeEffects();
            
            StartCoroutine(BonusСheckСoroutine());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        #endregion

        #region Functionality

        private void MakeEffects()
        {
            effects = new List<Effect>
            {
                new (EffectType.Buff, EffectTargetType.GamePoints, stats.GamePointsEffectDuration, stats.GamePointsEffectValue),
                new (EffectType.Buff, EffectTargetType.UnitHp, stats.UnitHpEffectDuration, stats.UnitHpEffectMultiplier),
                new (EffectType.Buff, EffectTargetType.UnitSpeed, stats.SpeedEffectDuration, stats.SpeedEffectMultiplier),
                new (EffectType.Debuff, EffectTargetType.GamePoints, stats.GamePointsEffectDuration, stats.GamePointsEffectValue),
                new (EffectType.Debuff, EffectTargetType.UnitHp, stats.UnitHpEffectDuration, stats.UnitHpEffectMultiplier),
                new (EffectType.Debuff, EffectTargetType.UnitSpeed, stats.SpeedEffectDuration, stats.SpeedEffectMultiplier),
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

                IBonusRepresentable newBonus = default;
                GameObject newBonusObject = default;
                
                newBonusObject = Instantiate(type == EffectType.Buff ? positiveBonusPrefab : negativeBonusPrefab, randomPoint.position, Quaternion.identity);
                
                switch (type)
                {
                    case EffectType.Buff:
                        newBonus = newBonusObject.AddComponent<PositiveBonus>();
                        SetUpBonus(type, ref newBonus, randomPoint);
                        _positiveBonuses.Add(new BonusItem(randomPoint, newBonus));
                        break;
                    case EffectType.Debuff:
                    
                        newBonus = newBonusObject.AddComponent<NegativeBonus>();
                        SetUpBonus(type, ref newBonus, randomPoint); 
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
        
        private void SetUpBonus(EffectType effectType, ref IBonusRepresentable bonus, Transform randomPoint)
        {
            var random = new System.Random();
            var effect = effects.Where(ef => ef.Type == effectType)
                .OrderBy(x => random.Next())
                .First();

            BonusType bonusType = default;
            BoosterType? boosterType = null;

            switch (effect.EffectTarget)
            {
                case EffectTargetType.GamePoints:
                    bonusType = effect.Type == EffectType.Buff ? BonusType.Gift : BonusType.Theft;
                    break;
                case EffectTargetType.UnitHp:
                    bonusType = effect.Type == EffectType.Buff ? BonusType.Booster : BonusType.SuddenDeath;
                    boosterType = effect.Type == EffectType.Buff ? BoosterType.TempInvulnerability : null;
                    break;
                case EffectTargetType.UnitSpeed:
                    bonusType = effect.Type == EffectType.Buff ? BonusType.Booster : BonusType.TempSlowdown;
                    boosterType = effect.Type == EffectType.Buff ? BoosterType.TempSpeedBoost: null;
                    break;
            }
            
            bonus.Init(bonusType, effect, randomPoint, boosterType);
        }
        
        private void OnGettingBonusNotify(IBonusRepresentable bonus, string tagElement)
        {
            if (string.Equals(tagElement, GameData.PlayerTag))
            {
                bonusEvent.Notify(new BonusArgs(GameData.PlayerTag, bonus.Effect));
            }
            
            StartCoroutine(RemoveBonusCoroutine(bonus));
        }

        private IEnumerator RemoveBonusCoroutine(IBonusRepresentable bonus)
        {
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

            Destroy(bonusGameObject.gameObject);

            yield return new WaitForSeconds(stats.DelayAfterRemove);
            
            collection.Remove(existingElement);
            
            yield return null;
        }
        #endregion
    }
}