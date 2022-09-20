using System;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Bonuses
{
    public abstract class EffectFactory : IEquatable<EffectFactory>
    {
        #region Fields

        private readonly Guid _id = new ();
        protected readonly float Power;
        protected readonly float Duration;
        
        #endregion

        #region Constructor
        
        protected EffectFactory(float power, float duration )
        {
            Power = power;
            Duration = duration;
        }
        
        #endregion
        
        #region Functionality
        
        public bool Equals(EffectFactory other)
        {
            return other switch
            {
                null => false,
                not null => _id == other._id
            };
        }
        
        // Factory method
        public abstract IEffectable GetEffect();

        #endregion
    }
}