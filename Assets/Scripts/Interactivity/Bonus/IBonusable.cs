using System;
using RollABall.Interactivity.Effects;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Bonuses
{
    public interface IBonusable : IEquatable<IBonusable>, IInteractable<IBonusable>, IDisposable
    {
        #region Properties
        Guid Id { get; }
        BonusType BonusType { get; }
        BoosterType? BoosterType { get; set; }
        Transform Point { get; }
        public IEffectable Effect { get; set; }
        #endregion
        
        #region Functionality
        
        bool IEquatable<IBonusable>.Equals(IBonusable other)  
        {        
            return other switch
            {
                null => false,
                not null => Id == other.Id && Point == other.Point
            };
        }  
        
        void Init(BonusType bonusType, IEffectable effect, Transform point, BoosterType? boosterType = null);
        #endregion
    }
}