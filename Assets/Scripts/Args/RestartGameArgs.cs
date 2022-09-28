using System;

// ReSharper disable once CheckNamespace
namespace RollABall.Args
{
    public class CurrentGameArgs : EventArgs
    {
        #region Properties

        public bool IsRestartGame { get; }
        
        #endregion
        
        #region Constructors
        
        public CurrentGameArgs(bool isRestartGame)
        {
            IsRestartGame = isRestartGame;
        }
        
        #endregion
    }
}