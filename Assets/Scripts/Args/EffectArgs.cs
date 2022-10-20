using System;
using RollABall.Interactivity.Bonuses;
using RollABall.Interactivity.Effects;
using UnityEngine.PlayerLoop;

// ReSharper disable once CheckNamespace
namespace RollABall.Args
{
    public class EffectArgs : EventArgs
    {
        #region Properties
        
        public EffectType EffectType { get;}
        public EffectTargetType EffectTargetType  { get;}
        public float? Power  { get; private set; }
        public bool? Increase   { get; private set; }
        public bool? IsInvulnerable   { get; private set; }
        public bool? CancelEffect   { get; private set; }
        
        #endregion
        
        #region Constructors 
        
        public EffectArgs(EffectType effectType, EffectTargetType effectTargetType)
        {
            EffectType = effectType;
            EffectTargetType = effectTargetType;
        }
        
        #endregion
        
        #region Functionality
        
        public void Init(float? power = null, bool? increase = null, bool? isInvulnerable = null,  bool? cancelEffect = null)
        {
            Power = power;
            Increase = increase;
            IsInvulnerable = isInvulnerable;
            CancelEffect = cancelEffect;
        }
        
        #endregion
    }
}