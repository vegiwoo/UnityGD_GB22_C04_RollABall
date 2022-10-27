using System.Collections.Generic;
using RollABall.Args;

// ReSharper disable once CheckNamespace
namespace RollABall.Infrastructure
{
    /// <summary>
    /// Describes behavior of data model when changing game states.
    /// </summary>
    internal interface IActionable
    {
        #region Properties 
        
        /// <summary>
        /// Data model status describing its state
        /// </summary>
        List<ISavableArgs> SavedState { get; set; }
        
        #endregion
        
        #region Functionality
        
        /// <summary>
        /// Called when a new game starts.
        /// </summary>
        void NewGameAction();
        
        /// <summary>
        /// Called when game is saved.
        /// </summary>
        void SaveGameAction();
        
        /// <summary>
        /// Called when game is loaded.
        /// </summary>
        /// <param name="args"></param>
        void LoadGameAction(SaveGameArgs args);
        
        /// <summary>
        /// Called when game is restarted.
        /// </summary>
        void RestartGameAction();

        /// <summary>
        /// Called when game is over.
        /// </summary>
        void LostGameAction();
        
        /// <summary>
        /// Called when game is won.
        /// </summary>
        void WonGameAction();

        #endregion
    }
}