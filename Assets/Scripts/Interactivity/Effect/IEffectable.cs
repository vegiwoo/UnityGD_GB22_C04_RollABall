#nullable enable
using System;
using RollABall.Interactivity.Effects;

// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Bonuses
{
    public interface IEffectable : IEquatable<Effect>
    {
        #region Properties
        
        Guid Id { get; }
        EffectType Type { get; }
        EffectTargetType EffectTarget { get; }
        BoosterType? BoosterType { get; } 
        float Power { get; }
        float Duration { get; }
        
        #endregion

        #region Functionality
        
        public string ToString()
        {
            return $"Effect: type {Type.ToString()} (target {EffectTarget.ToString()}), power: {Power}, duration: {Duration}";
        }
        
        bool IEquatable<Effect>.Equals(Effect? other)
        {
            return other switch
            {
                null => false,
                not null => Id == other.Id && Type == other.Type
            };
        }  

        #endregion
    }
}