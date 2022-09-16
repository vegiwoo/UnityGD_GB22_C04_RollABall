using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        #region Links

        [SerializeField] private BonusManagerStats stats;
        [SerializeField] private GameObject positiveBonusPrefab;
        [SerializeField] private GameObject negativeBonusPrefab;

        #endregion
        
        // TODO: Бонусы в одном классе с 2 мя списками (+ и -), индексаторы для этих списков
        
        #region Constant and variables
        
        private List<IBonusRepresentable> _positiveBonuses;
        private int _requiredNumberPositiveBonuses;
        
        private List<IBonusRepresentable> _negativeBonuses;
        private int _requiredNumberNegativeBonuses;

        private List<Effect> effects;

        #endregion
        
        #region Properties

        [SerializeField, Tooltip("Points on playing field for placing bonuses")] 
        private Transform[] bonusPoints;

        #endregion
        
        #region MonoBehaviour methods

        private void Start()
        {
            var halfOff = Math.DivRem(bonusPoints.Length, 2, out var remainder);
            _requiredNumberPositiveBonuses = halfOff;
            _requiredNumberNegativeBonuses = halfOff + remainder;
            
            _positiveBonuses = new List<IBonusRepresentable>(_requiredNumberPositiveBonuses);
            _negativeBonuses = new List<IBonusRepresentable>(_requiredNumberNegativeBonuses);
            
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
                        SetBonus(type, ref newBonus, randomPoint);
                        _positiveBonuses.Add(newBonus);
                        break;
                    case EffectType.Debuff:
                    
                        newBonus = newBonusObject.AddComponent<NegativeBonus>();
                        SetBonus(type, ref newBonus, randomPoint); 
                        _negativeBonuses.Add(newBonus);
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

        private void SetBonus(EffectType effectType, ref IBonusRepresentable bonus, Transform randomPoint)
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
            print($"{tagElement} получен бонус {bonus.PointOfPlacement}");
            
            // Понять что за тег - разложить по элементам
            // Оказать влияние на перса (опубликовать событие)
            // Удалить игровой объект со сцены
            // Удалить объект из коллекции

        }
        #endregion
    }
}