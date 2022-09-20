using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Bonuses
{
    public class BonusItem : IEquatable<BonusItem>
    {
        public Transform Point { get; }
        public IBonusable Bonus { get; }

        public BonusItem(Transform point, IBonusable bonus)
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

