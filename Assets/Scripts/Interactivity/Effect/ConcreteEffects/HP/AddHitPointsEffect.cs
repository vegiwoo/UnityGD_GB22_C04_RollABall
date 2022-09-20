using System;
using RollABall.Interactivity.Effects;

// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Bonuses
{
    public class AddHitPointsEffect : IEffectable
    {
        #region Properties
        
        public Guid Id { get; } = new ();
        public EffectType Type  => EffectType.Buff;
        public EffectTargetType EffectTarget => EffectTargetType.HitPoints;
        public BoosterType? BoosterType => Bonuses.BoosterType.TempInvulnerability;
        public float Power { get; }
        public float Duration { get; }
        
        #endregion

        #region Constructors
        
        public AddHitPointsEffect(float power, float duration)
        {
            Power = power;
            Duration = duration;
        }
        
        #endregion
    }
}