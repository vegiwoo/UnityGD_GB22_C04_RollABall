// ReSharper disable once CheckNamespace

using UnityEngine;

namespace RollABall.Interactivity.Bonuses
{
    public interface IBonusRepresentable
    {
        #region Properties
        
        BonusType Type { get;}
        NegativeBonusType? NegativeType { get; }
        PositiveBonusType? PositiveType { get; set; }
        BoosterType? BoosterType { get; set; }
        
        Transform PointOfPlacement { get; }
        float Power { get; }

        void PositiveInit(PositiveBonusType positiveType, BoosterType? boosterType, Transform pointOfPlacement, float power);
        void NegativeInit(NegativeBonusType negativeType, Transform point, float power);

        #endregion
    }
}