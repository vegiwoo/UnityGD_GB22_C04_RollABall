#nullable enable

using System;
using RollABall.Interactivity.Bonuses;

// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Effects
{
    /// <summary>
    /// Formed when a bonus (buff or debuff) affects the character
    /// </summary>
    public class Effect : IEffectable
    {
        #region Properties

        public Guid Id { get; } = new ();
        public EffectType Type { get; }
        public EffectTargetType EffectTarget { get; }
        public BoosterType? BoosterType { get; }
        public float Power { get; }
        public float Duration { get; }
        
        #endregion

        #region Constructors
        
        public Effect(EffectType type, EffectTargetType target, float duration, float power, BoosterType? boosterType = null)
        {
            Type = type;
            EffectTarget = target;
            Duration = duration;
            Power = power;
            BoosterType = boosterType;
        }
        
        #endregion
        
        #region Functionality

        public override string ToString()
        {
            return $"Effect: type {Type.ToString()} (target {EffectTarget.ToString()}), power: {Power}, duration: {Duration}";
        }
        
        #endregion
    }
}