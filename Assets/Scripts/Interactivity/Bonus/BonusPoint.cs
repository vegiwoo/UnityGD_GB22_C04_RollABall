using System;
using GameDevLib.Helpers;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Bonuses
{
    [Serializable]
    public class BonusPoint
    {
        [field: SerializeField] public Transform Point { get; set; }
        [field: SerializeField, ReadonlyField] public bool IsReserved { get; private set; }
        [field: SerializeField, ReadonlyField] public bool IsBonusPlaced{ get; private set; }

        public void Reserve()
        {
            IsReserved = true;
        }

        public void Clear()
        {
            IsReserved = IsBonusPlaced = false;
        }
    }
}