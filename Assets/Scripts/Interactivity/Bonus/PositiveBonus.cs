using GameDevLib.Helpers;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Bonuses
{
    public class PositiveBonus : InteractiveObject, IBonusRepresentable
    {
        #region Properties
        
        public BonusType Type { get; } = BonusType.Positive;
        public NegativeBonusType? NegativeType { get; } = null;
        
        [field:SerializeField,ReadonlyField]
        public PositiveBonusType? PositiveType { get; set; }
        
        [field:SerializeField,ReadonlyField]
        public BoosterType? BoosterType { get; set; }
        
        public Transform PointOfPlacement { get; private set; }
        
        [field:SerializeField, ReadonlyField]
        public float Power { get; set; }
        
        #endregion
  
        #region Constructors
        
        public void PositiveInit(PositiveBonusType positiveType, BoosterType? boosterType, Transform pointOfPlacement, float power)
        {
            PositiveType = positiveType;
            BoosterType = boosterType;
            PointOfPlacement = pointOfPlacement;
            Power = power;
        }
        
        public void NegativeInit(NegativeBonusType negativeType, Transform point, float power)
        {
            // Not implement
        }

        #endregion
        
        #region Functionality
        
        protected override void Interaction()
        {
       
        }
        
        #endregion
    }
}