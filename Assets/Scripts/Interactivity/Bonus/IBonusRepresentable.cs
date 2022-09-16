using UnityEngine;

// ReSharper disable once CheckNamespace
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
        
        /// <summary>
        /// Bonus receive event delegate.
        /// </summary>
        delegate void GettingBonusHandler(IBonusRepresentable bonus);
        
        /// <summary>
        /// Bonus Receipt Event.
        /// </summary>
        event GettingBonusHandler GettingNotify;
        
        #endregion
        
        #region Functionality
        void PositiveInit(PositiveBonusType positiveType, Transform point);
        void NegativeInit(NegativeBonusType negativeType, Transform point);

        /// <summary>
        /// Triggers a bonus receive event.
        /// </summary>
        /// <param name="bonus">Received bonus</param>
        void OnGettingNotify(IBonusRepresentable bonus);
        #endregion
    }
}