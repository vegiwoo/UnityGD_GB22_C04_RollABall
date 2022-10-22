using System;

// ReSharper disable once CheckNamespace
namespace RollABall.Args
{
    public class CurrentGameArgs : EventArgs
    {
        #region Properties

        public bool IsRestartGame { get; }
        public bool IsSaveGame { get; }
        public SaveGameArgs? SaveGameArgs { get; }
        
        public bool IsNeedState { get; }
        public (bool isLost, string message)? IsLostGame { get; }
        public (bool isWin, string message)? IsWinGame { get; }
        
        #endregion
        
        #region Constructors
        
        public CurrentGameArgs(bool isRestartGame, bool isSaveGame, bool isLoadGame, bool isNeedState,
            (bool isLost, string message)? isLostGame = null, 
            (bool isWin, string message)? isWinGame = null,
            SaveGameArgs? saveGameArgs = null)
        {
            IsRestartGame = isRestartGame;
            IsSaveGame = isSaveGame;
            SaveGameArgs = saveGameArgs;
            IsNeedState = isNeedState;
            IsLostGame = isLostGame;
            IsWinGame = isWinGame;
        }
        
        #endregion
    }
}