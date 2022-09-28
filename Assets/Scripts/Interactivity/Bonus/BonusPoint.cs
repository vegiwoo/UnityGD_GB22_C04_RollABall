using System;
using GameDevLib.Helpers;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Bonuses
{
    [Serializable]
    public class BonusPoint : IEquatable<BonusPoint>
    {
        #region Links
        
        [field: SerializeField] public Transform Point { get; set; }
        [field: SerializeField, ReadonlyField] public bool IsReserved { get; private set; }
        
        #endregion
        
        #region Constructors
        
        public void Reserve()
        {
            IsReserved = true;
        }
        
        #endregion

        #region Functionality
        
        public void Clear()
        {
            IsReserved = false;
        }

        public bool Equals(BonusPoint other)
        {
            return other switch
            {
                not null => other.Point == Point,
                _ => false
            };
        }
        #endregion
    }
}