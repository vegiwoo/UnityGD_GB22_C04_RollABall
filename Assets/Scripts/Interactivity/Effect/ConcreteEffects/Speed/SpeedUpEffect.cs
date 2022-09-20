using System;
using RollABall.Interactivity.Effects;

// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Bonuses
{
    public class SpeedUpEffect : IEffectable
    {
        #region Properties
        
        public Guid Id { get; } = new ();
        public EffectType Type  => EffectType.Buff;
        public EffectTargetType EffectTarget => EffectTargetType.UnitSpeed;
        public BoosterType? BoosterType => Bonuses.BoosterType.TempSpeedBoost;
        public float Power { get; }
        public float Duration { get; }
        
        #endregion

        #region Constructors
        
        public SpeedUpEffect(float power, float duration)
        {
            Power = power;
            Duration = duration;
        }
        
        #endregion
    }
}