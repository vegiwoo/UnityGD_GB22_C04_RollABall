using RollABall.Interactivity.Effects;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Bonuses
{
    public interface IBonusRepresentable : IInteractable<IBonusRepresentable>
    {
        #region Properties
        BonusType BonusType { get; }
        EffectType EffectType { get;}
        BoosterType? BoosterType { get; set; }
        Transform PointOfPlacement { get; }
        
        Effect EffectOfBonus { get; set; }

        #endregion
        
        #region Functionality

        void Init(BonusType bonusType, Effect effect, Transform point, BoosterType? boosterType = null);

        #endregion
    }
}