using System;
using RollABall.Interactivity.Bonuses;

// ReSharper disable once CheckNamespace
namespace RollABall.Args
{
    public class BonusArgs : EventArgs
    {
        #region Properties

        public string Tag { get; set; }
        public IEffectable Effect { get; set; }
        #endregion
        
        #region Constructors
        
        public BonusArgs(string tag, IEffectable effect)
        {
            Tag = tag;
            Effect = effect;
        }
        
        #endregion
    }
}

