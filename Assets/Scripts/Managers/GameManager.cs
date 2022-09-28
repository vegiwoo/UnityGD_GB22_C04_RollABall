using GameDevLib.Interfaces;
using RollABall.Args;
using RollABall.Events;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Debug;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    public class GameManager : BaseManager, IObserver<PlayerArgs>
    {
        #region Links
        
        [field:SerializeField] private PlayerEvent PlayerEvent { get; set; }

        #endregion

        #region MonoBehaviour methods
        
        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerEvent.Attach(this);
        }
        
        #endregion
        
        #region Functionality
        
        public override void Dispose()
        {
            base.Dispose();
            PlayerEvent.Detach(this);
        }
        
        // Event handler for PlayerEvent 
        public void OnEventRaised(ISubject<PlayerArgs> subject, PlayerArgs args)
        {
            var lost = args.CurrentHp <= 0;
            var win = args.GamePoints >= GameStats.GameHighScore;
            
            EditorApplication.isPaused = lost|| win;
            if (lost)
            {
                Log("You lose :(");
            } else if (win)
            {
                Log("You win :)");
            }
        }
        
        // Event handler for CurrentGameEvent 
        public override void OnEventRaised(ISubject<CurrentGameArgs> subject, CurrentGameArgs args)
        {
            // Do something...
        }
        
        #endregion
    }
}