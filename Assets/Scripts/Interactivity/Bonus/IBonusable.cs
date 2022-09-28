using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Bonuses
{
    public interface IBonusable : IEquatable<IBonusable>, IInteractable<IBonusable>, IDisposable
    {
        #region Properties

        Guid Id => new ();
        BonusPoint Point { get; set; }
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
        void Init(IEffectable effect, BonusPoint point)
        {
            Effect = effect;
            Point = point;
        }

        /// <summary>
        /// Returns game object (prefab) of bonus.
        /// </summary>
        /// <returns></returns>
        GameObject? GetBonusGameObject()
        {
            return Point.Point.gameObject.transform.GetChild(0).gameObject;
        }

        int GetChildrenCount()
        {
            return Point.Point.transform.childCount;
        }
        
        #endregion
    }
}