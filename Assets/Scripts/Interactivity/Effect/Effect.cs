using System;
using RollABall.Interactivity.Bonuses;
using RollABall.Stats;

// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Effects
{
    /// <summary>
    /// Formed when a bonus (buff or debuff) affects the character
    /// </summary>
    [Serializable]
    public class Effect
    {
        public EffectType Type { get; }
        public EffectTargetType EffectTarget { get; }
        public float Power { get; }
        public float Duration { get; }

        public Effect(EffectType type, EffectTargetType target, float duration, float power)
        {
            Type = type;
            EffectTarget = target;
            Duration = duration;
            Power = power;
        }
    }
}