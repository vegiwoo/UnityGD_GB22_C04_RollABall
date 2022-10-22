using System;
using Newtonsoft.Json;
using RollABall.Managers;

// ReSharper disable once CheckNamespace
namespace RollABall.Args
{
    [Serializable]
    public class EffectSaveArgs : ISavableArgs
    {
        #region Properties 
    
        /// <summary>
        /// An effect applied on the player that has a duration.
        /// </summary>
        public Effect AppliedEffect { get; }
        
        /// <summary>
        /// Remaining duration of applied effect.
        /// </summary>
        public float RemainingDuration { get; set; }

        #endregion
    
        #region Constructor
    
        [JsonConstructor]
        public EffectSaveArgs(Effect appliedEffect, float remainingDuration)
        {
            AppliedEffect = appliedEffect;
            RemainingDuration = remainingDuration;
        }

        #endregion
    
        #region Functionality

        public override string ToString()
        {
            return $"Effect:{AppliedEffect}, RemainingDuration:{RemainingDuration}";
        }

        #endregion
    }
}