using System;
using System.Collections.Generic;
using GameDevLib.Interfaces;
using RollABall.Args;
using RollABall.Events;
using RollABall.Infrastructure;
using RollABall.Models;
using UnityEngine;
using GameStats = RollABall.Stats.GameStats;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    /// <summary>
    /// Base manager for all other managers in game.
    /// </summary>
    public abstract class BaseManager : MonoBehaviour, IActionable, GameDevLib.Interfaces.IObserver<CurrentGameArgs>, IDisposable
    {
        #region Links
        
        [field: Header("Main stats")]
        [field: SerializeField] protected CurrentGameEvent GameEvent { get; set; }
        [field: Header("Main events")]
        [field: SerializeField] protected GameStats GameStats { get; set; }
        
        #endregion
        
        #region Properties

        public List<ISavableArgs> SavedState { get; set; }

        #endregion
        
        #region Filds
        
        protected readonly System.Random systemRandom = new ();
        
        #endregion
        
        #region MonoBehaviour methods

        protected virtual void Start()
        {
            NewGameAction();
        }
        
        protected virtual void OnEnable()
        {
            GameEvent.Attach(this);
        }

        protected virtual void OnDisable()
        {
            Dispose();
        }

        #endregion

        #region Functionality
        
        public abstract void NewGameAction();
        
        public abstract void SaveGameAction();
        
        public abstract void LoadGameAction(SaveGameArgs args);
        
        public abstract void RestartGameAction();

        public abstract void LostGameAction();

        public abstract void WonGameAction();
        
        public virtual void OnEventRaised(ISubject<CurrentGameArgs> subject, CurrentGameArgs args)
        {
            if (args.CurrentGameState.HasValue)
            {
                switch (args.CurrentGameState.Value)
                {
                    case CurrentGameState.Restart:
                        RestartGameAction();
                        break;
                    case CurrentGameState.Save:
                        SaveGameAction();
                        break;
                    case CurrentGameState.Load when args.SaveGameArgs is not null:
                        LoadGameAction(args.SaveGameArgs);
                        break;
                    case CurrentGameState.Lost:
                        LostGameAction();
                        break;
                    case CurrentGameState.Won:
                        WonGameAction();
                        break;
                }
            }
        }
        
        public virtual void Dispose()
        {
            GameEvent.Detach(this);
        }

        #endregion
    }
}