using System;

// ReSharper disable once CheckNamespace
namespace RollABall.Args
{
    public class CurrentGameArgs : EventArgs
    {
        #region Properties

        public bool IsRestartGame { get; }
        public (bool isLost, string message)? IsLostGame { get; }
        public (bool isWin, string message)? IsWinGame { get; }
        #endregion
        
        #region Constructors
        
        public CurrentGameArgs(bool isRestartGame, 
            (bool isLost, string message)? isLostGame = null, 
            (bool isWin, string message)? isWinGame = null)
        {
            IsRestartGame = isRestartGame;
            IsLostGame = isLostGame;
            IsWinGame = isWinGame;
        }
        
        #endregion
    }
}