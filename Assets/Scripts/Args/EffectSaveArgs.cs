using System;
using Newtonsoft.Json;
using RollABall.Managers;

// ReSharper disable once CheckNamespace
namespace RollABall.Args
{
    [Serializable]
    public class EffectSaveArgs : EventArgs, ISavableArgs
    {
        #region Properties 
    
        public Effect AppliedEffect { get; }
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