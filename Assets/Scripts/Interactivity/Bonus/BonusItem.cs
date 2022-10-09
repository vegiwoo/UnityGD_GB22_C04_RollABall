using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Bonuses
{
    public class BonusItem : IComparable<BonusItem>
    {
        public IBonusable Bonus { get; }
        public GameObject BonusGo { get; }
        
        public BonusItem(IBonusable bonus, GameObject bonusGo)
        {
            Bonus = bonus;
            BonusGo = bonusGo;
        }

        public int CompareTo(BonusItem other)
        {
            var selfPosition = BonusGo.transform.position;
            var otherPosition = other.BonusGo.transform.position;

            return selfPosition.x.Equals(otherPosition.x) && selfPosition.z.Equals(otherPosition.z) ? 
                0 : 
                selfPosition.x < otherPosition.x && selfPosition.z < otherPosition.z ? 
                    -1 : 
                    1;
        }
    }
}