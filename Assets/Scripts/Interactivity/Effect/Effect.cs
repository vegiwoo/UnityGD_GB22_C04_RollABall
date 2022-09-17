#nullable enable

using System;
using RollABall.Interactivity.Bonuses;

// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Effects
{
    /// <summary>
    /// Formed when a bonus (buff or debuff) affects the character
    /// </summary>
    public class Effect : IEquatable<Effect>
    {
        private Guid Id { get; } = new ();
        public EffectType Type { get; }
        public EffectTargetType EffectTarget { get; }
        
        public BoosterType? BoosterType { get; }
        public float Power { get; }
        public float Duration { get; }

        public Effect(EffectType type, EffectTargetType target, float duration, float power, BoosterType? boosterType = null)
        {
            Type = type;
            EffectTarget = target;
            Duration = duration;
            Power = power;
            BoosterType = boosterType;
        }
        
        #region Functionality

        public override string ToString()
        {
            return $"Effect: type {Type.ToString()}, target {EffectTarget.ToString()}, power: {Power}, duration: {Duration}";
        }

        public bool Equals(Effect? other)
        {
            if (other == null) return false;
            return Id == other.Id && Type == other.Type;  
        }  
        #endregion
    }
}