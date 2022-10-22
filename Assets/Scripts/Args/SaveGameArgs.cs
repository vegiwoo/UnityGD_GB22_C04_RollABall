using System;
using System.Collections.Generic;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace RollABall.Args
{
    [Serializable]
    public class SaveGameArgs : ISavableArgs
    {
        #region Properties
        
        public PlayerArgs PlayerArgs { get; set; }
        public List<EffectSaveArgs> EffectSaveArgs { get; set; }
        public List<BonusSaveArgs> BonusSaveArgs { get; set; }
        
        #endregion
        
        #region Constructors

        [JsonConstructor]
        public SaveGameArgs(PlayerArgs playerArgs,  List<EffectSaveArgs> effectSaveArgs, List<BonusSaveArgs> bonusSaveArgs)
        {
            PlayerArgs = playerArgs;
            EffectSaveArgs = effectSaveArgs;
            BonusSaveArgs = bonusSaveArgs;
        }

        public SaveGameArgs() { }

        #endregion
        
        #region Functionality

        public override string ToString()
        {
            return $"PlayerArgs:{PlayerArgs}, EffectSaveArgs:{EffectSaveArgs}, BonusSaveArgs:{BonusSaveArgs}";
        }

        #endregion
    }
}