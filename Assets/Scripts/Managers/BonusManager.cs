using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RollABall.Interactivity.Bonuses;
using UnityEngine;
using static UnityEngine.Debug;
using Random = UnityEngine.Random;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    public class BonusManager : MonoBehaviour
    {
        #region Links

        [SerializeField] private GameObject positiveBonusPrefab;
        [SerializeField] private GameObject negativeBonusPrefab;
        
        #endregion
        
        
        #region Constant and variables
        
        private List<IBonusRepresentable> _positiveBonuses;
        private int _requiredNumberPositiveBonuses;
        
        private List<IBonusRepresentable> _negativeBonuses;
        private int _requiredNumberNegativeBonuses;
        
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
                .Where(point => _positiveBonuses.Count(el => el.PointOfPlacement == point) == 0 && 
                                _negativeBonuses.Count(el => el.PointOfPlacement) == 0)
                .ToList();
        }
        
        private IEnumerator BonusСheckСoroutine()
        {
            while (true)
            {
                var freePoints = FindFreePoints();
                if(freePoints.Count == 0) continue;
                
                if (_positiveBonuses.Count != _requiredNumberPositiveBonuses)
                {
                    var numberOfAddition = _requiredNumberPositiveBonuses - _positiveBonuses.Count;
                    
                    // Вынести в отдельную корутину с задержкой
                    for (int i = 0; i < numberOfAddition; i++)
                    {
                        var randomIndex = Random.Range(0, freePoints.Count - 1);
                        var randomPoint = freePoints[randomIndex];

                        var newBonusObject = Instantiate(positiveBonusPrefab, randomPoint.position, Quaternion.identity);
                        var newBonus = newBonusObject.AddComponent<PositiveBonus>();
                        newBonus.PositiveInit(PositiveBonusType.GamePoints, null, randomPoint, 10);
                        
                        _positiveBonuses.Add(newBonus);

                        freePoints.Remove(randomPoint);
                    }
                }
                
                if (_negativeBonuses.Count != _requiredNumberNegativeBonuses)
                {
                    // Найти пустые точки и добавить негативные бонусы 
                }

                yield return new WaitForSeconds(20);
                
                yield return null;
            }
        }
        
        #endregion
    }
}


// Бонусы в одном классе с 2 мя списками (+ и -), индексаторы для этих списков