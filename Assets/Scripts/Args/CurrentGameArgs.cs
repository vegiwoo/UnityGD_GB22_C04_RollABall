#nullable enable

using System;
using RollABall.Models;

// ReSharper disable once CheckNamespace
namespace RollABall.Args
{
    public class CurrentGameArgs : EventArgs
    {
        #region Properties

        public CurrentGameState? CurrentGameState { get; }
      
        public SaveGameArgs? SaveGameArgs { get; }
        public (bool isLost, string message)? IsLostGame { get; }
        public (bool isWin, string message)? IsWinGame { get; }
        
        #endregion
        
        #region Constructors
        
        public CurrentGameArgs(CurrentGameState? currentGameState,
            SaveGameArgs? saveGameArgs,
            (bool isLost, string message)? isLostGame = null, 
            (bool isWin, string message)? isWinGame = null)
        {
            CurrentGameState = currentGameState;
            SaveGameArgs = saveGameArgs;
            IsLostGame = isLostGame;
            IsWinGame = isWinGame;
        }
        
        #endregion
    }
}