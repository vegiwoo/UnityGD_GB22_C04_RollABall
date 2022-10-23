using System;
using System.Collections.Generic;
using System.Linq;
using GameDevLib.Interfaces;
using RollABall.Args;
using RollABall.Events;
using RollABall.Infrastructure.Memento;
using RollABall.ScriptableObjects;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    public class GameManager : BaseManager, GameDevLib.Interfaces.IObserver<PlayerArgs>, 
        GameDevLib.Interfaces.IObserver<List<ISavableArgs>>, IMementoOrganizer<List<ISavableArgs>>
    {
        #region Links
        
        [field:SerializeField] private PlayerEvent PlayerEvent { get; set; }
        [field: SerializeField] private GameCaretaker GameCaretaker { get; set; }

        #endregion

        #region MonoBehaviour methods
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            PassStateEvent.Attach(this);
            PlayerEvent.Attach(this);
        }
        
        #endregion
        
        #region Functionality

        protected override void InitManager(InitItemMode mode)
        {
            GameCaretaker.Init(this, "SavedGames", "SavedGame_");
            State = new List<ISavableArgs>(new List<SaveGameArgs>()) { new SaveGameArgs() };
        }

        // Event handlers
        public void OnEventRaised(ISubject<PlayerArgs> subject, PlayerArgs args)
        {
            if (args.CurrentHp <= 0)
            {
                GameEvent.Notify(new CurrentGameArgs(false, false, false, (true, "You have spent all your hit points :("), null));
            } else if (args.GamePoints >= GameStats.GameHighScore)
            {
                GameEvent.Notify(new CurrentGameArgs(false, false,  false, (true, "You have reached required number of points :)")));
            }
        }
        
        public override void OnEventRaised(ISubject<CurrentGameArgs> subject, CurrentGameArgs args)
        {
            if (args.IsSaveGame)
            {
                GameCaretaker.Save();
            }
            
            if (args.IsLoadGame)
            {
                GameCaretaker.Load();
            }
        }
        
        public void OnEventRaised(ISubject<List<ISavableArgs>> subject, List<ISavableArgs> args)
        {
            try
            {
                var playerSaveArgs = args.Cast<PlayerArgs>().First();
                ((SaveGameArgs)State[0]).PlayerArgs = playerSaveArgs;
            }
            catch (Exception e)
            {
                // ignored
            } 
            
            try
            {
                var effectSaveArgs = args.Cast<EffectSaveArgs>().ToList();
                ((SaveGameArgs)State[0]).EffectSaveArgs = effectSaveArgs;
            }
            catch (Exception e)
            {
                // ignored
            }
            
            try
            {
                var bonusSaveArgs = args.Cast<BonusSaveArgs>().ToList();
                ((SaveGameArgs)State[0]).BonusSaveArgs = bonusSaveArgs;
            }
            catch (Exception e)
            {
                // ignored
            }
        }
        
        // Memento pattern
        
        public IMemento<List<ISavableArgs>> Save()
        {
            return new Memento<List<ISavableArgs>>(State, "Game");
        }

        public void Load(IMemento<List<ISavableArgs>> memento)
        {
            if (memento is not Memento<List<ISavableArgs>>)
            {
                throw new Exception("Unknown memento class " + memento.ToString());
            }

            State = memento.State;
            GameEvent.Notify(new CurrentGameArgs(false, false, true, null, null, State.First() as SaveGameArgs));
        }
        
        
        public override void Dispose()
        {
            base.Dispose();
            
            PassStateEvent.Detach(this);
            PlayerEvent.Detach(this);
        }

        #endregion
    }
}