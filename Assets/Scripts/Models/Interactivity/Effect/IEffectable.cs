#nullable enable

using System;
using RollABall.Interactivity.Effects;

// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Bonuses
{
    public interface IEffectable : IEquatable<IEffectable>
    {
        #region Properties
        Guid Id => Guid.NewGuid();
        EffectType Type { get; }
        EffectTargetType EffectTarget { get; }
        BoosterType BoosterType { get; }
        float NegativePower { get; }
        float PositivePower { get; }
        float Duration { get; }
        
        #endregion

        #region Functionality

        bool IEquatable<IEffectable>.Equals(IEffectable? other)
        {
            return other switch
            {
                null => false,
                not null => Id == other.Id && Type == other.Type && EffectTarget == other.EffectTarget
            };
        }  

        #endregion
    }
}