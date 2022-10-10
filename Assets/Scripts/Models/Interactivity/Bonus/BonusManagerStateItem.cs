using System;
using RollABall.Managers;
using RollABall.Models;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Bonuses
{
    [Serializable]
    public class BonusManagerStateItem
    {
        #region Properties
    
        public Point Point { get; }
        public Effect Effect { get; }
        public bool IsActive { get; }
    
        #endregion
    
        #region Constructors

        public BonusManagerStateItem(Point point, Effect effect, bool isActive)
        {
            Point = point;
            Effect = effect;
            IsActive = isActive;
        }
    
        public BonusManagerStateItem(Vector3 position, Effect effect, bool isActive)
        {
            Effect = effect;
            Point = new Point(position);
            IsActive = isActive;
        }
        
        #endregion
    }
}