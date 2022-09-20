using System;
using RollABall.Interactivity.Effects;

// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Bonuses
{
    public class AddGamePointsEffect : IEffectable
    {
        #region Properties
        
        public Guid Id { get; } = new ();
        public EffectType Type  => EffectType.Buff;
        public EffectTargetType EffectTarget => EffectTargetType.GamePoints;
        public BoosterType? BoosterType => null;
        public float Power { get; }
        public float Duration { get; }
        
        #endregion

        #region Constructors
        
        public AddGamePointsEffect(float power, float duration)
        {
            Power = power;
            Duration = duration;
        }
        
        #endregion
    }
}