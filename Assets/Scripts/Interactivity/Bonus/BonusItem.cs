using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Bonuses
{
    internal class BonusItem : IEquatable<BonusItem>
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
            if (other == null) return false;
            return Point == other.Point && Bonus == other.Bonus;  
        }  
    }
}

