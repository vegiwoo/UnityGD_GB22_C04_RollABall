using GameDevLib.Interfaces;
using RollABall.Args;
using RollABall.Events;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    public partial class GameManager : BaseManager, IObserver<PlayerArgs>
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
        }

        #endregion
    }
}

// public IMemento<PlayerArgs> Save()
// {
//     State = MakeState();
//     return new Memento<PlayerArgs>(State, "Player");
// }
//
// public void Load(IMemento<PlayerArgs> memento)
// {
//     if (memento is not Memento<PlayerArgs>)
//     {
//         throw new Exception("Unknown memento class " + memento.ToString());
//     }
//     
//     State = memento.State;
//     InitPlayer(true);
// }
        
// // Memento pattern - init caretaker for organizer.
// Caretaker.Init(this, "Player", "PlayerMemento");