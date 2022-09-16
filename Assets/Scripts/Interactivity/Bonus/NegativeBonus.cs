using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Bonuses
{
    public class NegativeBonus : InteractiveObject, IBonusRepresentable
    {
        #region Properties
        #endregion
        
        public BonusType Type { get; } = BonusType.Negative;
        public NegativeBonusType? NegativeType { get; private set; }
        public PositiveBonusType? PositiveType { get; set; } = null;
        public BoosterType? BoosterType { get; set; } = null;
        public Transform PointOfPlacement { get; private set; }
        public float Power { get; private set; }

        #region Functionality
        
        public void NegativeInit(NegativeBonusType negativeType, Transform point, float power)
        {
            NegativeType = negativeType;
            PointOfPlacement = point;
            Power = power;
        }

        public void PositiveInit(PositiveBonusType positiveType, BoosterType? boosterType, Transform pointOfPlacement,
            float power)
        {
            // .. Not implement
        }
        
        protected override void Interaction()
        {
            //...
        }
        
        #endregion
    }
}