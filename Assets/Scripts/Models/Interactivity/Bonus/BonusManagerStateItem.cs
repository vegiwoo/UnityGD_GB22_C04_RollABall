using System;
using Newtonsoft.Json;
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
    
        public Point Point { get; set; }
        public Effect Effect { get; set; }
        public bool IsActive { get; set; }
    
        #endregion
    
        #region Constructors

        [JsonConstructor]
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

        public override string ToString()
        {
            return $"Point:{Point}, Effect:{Effect}, IsActive:{IsActive}";
        }

        #endregion
    }
}