using GameDevLib.Interfaces;
using RollABall.Args;
using RollABall.Events;
using UnityEngine;

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

        protected override void InitManager()
        {
            // Do something ...
        }

        // Event handler for PlayerEvent 
        public void OnEventRaised(ISubject<PlayerArgs> subject, PlayerArgs args)
        {
            if (args.CurrentHp <= 0)
            {
                GameEvent.Notify(new CurrentGameArgs(false, false, false, (true, "You have spent all your hit points :("), null));
            } else if (args.GamePoints >= GameStats.GameHighScore)
            {
                GameEvent.Notify(new CurrentGameArgs(false, false, false,  null, (true, "You have reached required number of points :)")));
            }
        }
        
        // Event handler for CurrentGameEvent 
        public override void OnEventRaised(ISubject<CurrentGameArgs> subject, CurrentGameArgs args)
        {
            // Do something...
        }
        
        public override void Dispose()
        {
            base.Dispose();
            PlayerEvent.Detach(this);
        }

        #endregion
    }
}