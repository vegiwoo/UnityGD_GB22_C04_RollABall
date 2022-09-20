using System;
using RollABall.Interactivity.Effects;

// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Bonuses
{
    public class SlowDownEffect : IEffectable
    {
        #region Properties
        
        public Guid Id { get; } = new ();
        public EffectType Type  => EffectType.Debuff;
        public EffectTargetType EffectTarget => EffectTargetType.UnitSpeed;
        public BoosterType? BoosterType => null;
        public float Power { get; }
        public float Duration { get; }
        
        #endregion

        #region Constructors
        
        public SlowDownEffect(float power, float duration)
        {
            Power = power;
            Duration = duration;
        }
        
        #endregion
    }
}