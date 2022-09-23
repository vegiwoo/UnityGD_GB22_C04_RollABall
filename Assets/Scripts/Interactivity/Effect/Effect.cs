using System;
using RollABall.Interactivity.Bonuses;
using RollABall.Interactivity.Effects;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    [Serializable]
    public class Effect : IEffectable
    {
        #region Properties
        [field: SerializeField] public EffectTargetType EffectTarget { get; set; }
        [field: SerializeField] public EffectType Type { get; set; }
        [field: SerializeField] public BonusType BonusType { get; set; }
        [field: SerializeField] public BoosterType BoosterType { get; set; }
        [field: SerializeField] public float NegativePower { get; set; }
        [field: SerializeField] public float PositivePower { get; set; }
        [field: SerializeField] public float Duration { get; set; }
        
        #endregion
        
        #region Functionality
        
        public override string ToString()
        {
            return $"Effect: type {Type.ToString()} (target {EffectTarget.ToString()}), power: - { NegativePower} | + {PositivePower}, duration: {Duration}";
        }
        
        #endregion
    }
}