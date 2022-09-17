using RollABall.Interactivity.Effects;

// ReSharper disable once CheckNamespace
namespace RollABall.Args
{
    public struct BonusArgs
    {
        #region Properties 
        public string Tag { get; set; }
        public Effect Effect { get; set; }
        #endregion
        
        #region Constructors
        public BonusArgs(string tag, Effect effect)
        {
            Tag = tag;
            Effect = effect;
        }
        #endregion
    }
}

