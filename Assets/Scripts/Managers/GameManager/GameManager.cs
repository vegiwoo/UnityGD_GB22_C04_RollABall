using GameDevLib.Interfaces;
using RollABall.Args;
using RollABall.Events;
using RollABall.Models;
using RollABall.ScriptableObjects;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    public partial class GameManager : BaseManager, IObserver<PlayerArgs>
    {
        #region Links
        [field: Header("Other events")]
        [field:SerializeField] private PlayerEvent PlayerEvent { get; set; }
        
        [field: Header("Memento")]
        [field: SerializeField] private GameCaretaker Caretaker { get; set; }

        #endregion

        #region MonoBehaviour methods
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            PlayerEvent.Attach(this);
            SaveGameEvent.Attach(this);
        }
        
        #endregion
        
        #region Functionality

        // Event handler for PlayerEvent 
        public void OnEventRaised(ISubject<PlayerArgs> subject, PlayerArgs args)
        {
            if (args.CurrentHp <= 0)
            {
                var currentGameArgs = new CurrentGameArgs(CurrentGameState.Lost, null);
                GameEvent.Notify(currentGameArgs);
            } 
            else if (args.GamePoints >= GameStats.GameHighScore)
            {
                var currentGameArgs = new CurrentGameArgs(CurrentGameState.Won, null);
                GameEvent.Notify(currentGameArgs);
            
            
                throw new System.NotImplementedException();
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            
            PlayerEvent.Detach(this);
            SaveGameEvent.Detach(this);
        }

        #endregion
    }
}