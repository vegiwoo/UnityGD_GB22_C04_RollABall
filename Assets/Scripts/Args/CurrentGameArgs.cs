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

        #endregion
        
        #region Constructors

        public CurrentGameArgs(CurrentGameState? currentGameState, SaveGameArgs? saveGameArgs)
        {
            CurrentGameState = currentGameState;
            SaveGameArgs = saveGameArgs;
        }

        #endregion
    }
}