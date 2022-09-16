using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RollABall.Interactivity.Bonuses;
using UnityEngine;
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
                if (_requiredNumberPositiveBonuses != _positiveBonuses.Count)
                {
                    var numberOfAddition = _requiredNumberPositiveBonuses - _positiveBonuses.Count;
                    yield return StartCoroutine(BonusPlacingСoroutine(BonusType.Positive, numberOfAddition));
                }
  
                if (_negativeBonuses.Count != _requiredNumberNegativeBonuses)
                {
                    var numberOfAddition = _requiredNumberNegativeBonuses - _negativeBonuses.Count;
                    yield return StartCoroutine(BonusPlacingСoroutine(BonusType.Negative, numberOfAddition));
                }

                yield return null;
            }
        }
        
        private IEnumerator BonusPlacingСoroutine(BonusType type, int count)
        {
            var freePoints = FindFreePoints();
            
            var counter = count;
            while (counter != 0 && freePoints.Count > 0)
            {
                var randomIndex = Random.Range(0, freePoints.Count - 1);
                var randomPoint = freePoints[randomIndex];

                GameObject newBonusObject = default;
                IBonusRepresentable newBonus;

                switch (type)
                {
                    case BonusType.Positive:
                        newBonusObject = Instantiate(positiveBonusPrefab, randomPoint.position, Quaternion.identity);
                        newBonus = newBonusObject.AddComponent<PositiveBonus>();
                        newBonus.PositiveInit(PositiveBonusType.GamePoints, null, randomPoint, 10); // рандомно! 
                        _positiveBonuses.Add(newBonus);
                        break;
                    case BonusType.Negative:
                        newBonusObject = Instantiate(negativeBonusPrefab, randomPoint.position, Quaternion.identity);
                        newBonus = newBonusObject.AddComponent<NegativeBonus>();
                        newBonus.NegativeInit(NegativeBonusType.Wound, randomPoint, 10); // рандомно!
                        _negativeBonuses.Add(newBonus);
                        break;
                }

                if (newBonusObject != null) newBonusObject.transform.parent = transform;

                freePoints.Remove(randomPoint);
                --counter;
                
                yield return new WaitForSeconds(0.5f);
                yield return null;
            }
        }
        
        #endregion
    }
}


// Бонусы в одном классе с 2 мя списками (+ и -), индексаторы для этих списков