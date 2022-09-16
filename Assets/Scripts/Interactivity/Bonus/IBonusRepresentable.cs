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
        BonusActionPointType ActionPointType { get; set; }
        Transform PointOfPlacement { get; }
        float Power { get; }
        
        /// <summary>
        ///  Bonus duration in seconds
        /// </summary>
        float Duration { get; }

        void PositiveInit(PositiveBonusType positiveType, Transform point);
        void NegativeInit(NegativeBonusType negativeType, Transform point);

        #endregion
    }
}