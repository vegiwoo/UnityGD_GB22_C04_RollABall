#nullable enable

using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Args
{
    public class BonusManagerArgs : EventArgs
    {
        #region Properties
        public Transform[]? ActivatePoints { get; }
        public Transform? UnactivatePoint { get; }
        public Transform? ActivatePoint { get; }
        public bool RemoveAll { get; }
        #endregion
        
        #region Constructors

        public BonusManagerArgs(Transform[]? activatePoints, Transform? unactivatePoint, Transform? activatePoint, bool removeAll)
        { 
            ActivatePoints = activatePoints;
            UnactivatePoint = unactivatePoint;
            ActivatePoint = activatePoint;
            RemoveAll = removeAll;
        }

        #endregion
    }
}

