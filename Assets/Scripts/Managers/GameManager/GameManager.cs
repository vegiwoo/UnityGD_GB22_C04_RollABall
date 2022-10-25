using GameDevLib.Interfaces;
using RollABall.Args;
using RollABall.Events;
using RollABall.ScriptableObjects;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    public partial class GameManager : BaseManager, IObserver<PlayerArgs>
    {
        #region Links
        
        [field:SerializeField] private PlayerEvent PlayerEvent { get; set; }
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
                var currentGameArgs = new CurrentGameArgs(null, null, (true, "You have spent all your hit points :("));
                GameEvent.Notify(currentGameArgs);
                
            } else if (args.GamePoints >= GameStats.GameHighScore)
            {
                var currentGameArgs = new CurrentGameArgs(null, null, null, (true, "You have reached required number of points :)"));
                GameEvent.Notify(currentGameArgs);
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