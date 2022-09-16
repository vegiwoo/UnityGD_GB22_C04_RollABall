using System;
using GameDevLib.Helpers;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Bonuses
{
    public class NegativeBonus : InteractiveObject, IBonusRepresentable
    {
        #region Properties
        
        public BonusType Type { get; } = BonusType.Negative;
        
        [field: SerializeField, ReadonlyField]
        public NegativeBonusType? NegativeType { get; private set; }
        
        public PositiveBonusType? PositiveType { get; set; } = null;
        
        public BoosterType? BoosterType { get; set; } = null;
       
        public BonusActionPointType ActionPointType { get; set; }

        public Transform PointOfPlacement { get; private set; }
        
        public float Power { get; private set; }
        
        public float Duration { get; private set; }

        #endregion
        
        #region Functionality
        
        public void NegativeInit(NegativeBonusType negativeType, Transform point)
        {
            PointOfPlacement = point;
            NegativeType = negativeType.RandomValue(negativeType);

            switch (NegativeType)
            {
                case NegativeBonusType.TempSlowdown:
                    ActionPointType = BonusActionPointType.Speed;
                    Power = -2f;
                    Duration = 10f;
                    break;
                case NegativeBonusType.Wound:
                    ActionPointType = BonusActionPointType.GamePoints;
                    Power = -15f;
                    Duration = 0;
                    break;
                case NegativeBonusType.InstantDeath:
                    ActionPointType = BonusActionPointType.Hp;
                    Power = -1000f;
                    Duration = 0;
                    break;
            }
        }

        public void PositiveInit(PositiveBonusType positiveType, Transform point) { }
        
        protected override void Interaction()
        {
            //...
        }
        
        #endregion
    }
}