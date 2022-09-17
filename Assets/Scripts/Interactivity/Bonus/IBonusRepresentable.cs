using System;
using JetBrains.Annotations;
using RollABall.Interactivity.Effects;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Bonuses
{
    public interface IBonusRepresentable : IEquatable<IBonusRepresentable>, IInteractable<IBonusRepresentable>
    {
        #region Properties
        Guid Id { get; }
        BonusType BonusType { get; }
        EffectType EffectType { get;}
        BoosterType? BoosterType { get; set; }
        Transform Point { get; }
        public Effect Effect { get; set; }
        #endregion
        
        #region Functionality
        void Init(BonusType bonusType, Effect effect, Transform point, BoosterType? boosterType = null);
        #endregion
    }
}