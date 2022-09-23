using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Bonuses
{
    public interface IBonusable : IEquatable<IBonusable>, IInteractable<IBonusable>, IDisposable
    {
        #region Properties

        Guid Id => new ();
        Transform Point { get; set; }
        IEffectable Effect { get; set; }
        
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

        /// <summary>
        /// Initializes bonus.
        /// </summary>
        /// <param name="effect">Bonus effect.</param>
        /// <param name="point">Placement point.</param>
        void Init(IEffectable effect, Transform point)
        {
            Effect = effect;
            Point = point;
        }

        #endregion
    }
}