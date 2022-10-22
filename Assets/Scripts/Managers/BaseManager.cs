using System;
using System.Collections.Generic;
using GameDevLib.Interfaces;
using RollABall.Args;
using RollABall.Events;
using UnityEngine;
using GameStats = RollABall.Stats.GameStats;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    /// <summary>
    /// Base manager for all other managers in game.
    /// </summary>
    public abstract class BaseManager : MonoBehaviour, GameDevLib.Interfaces.IObserver<CurrentGameArgs>, IDisposable
    {
        #region Links

        [field: Header("Main Links")]
        [field: SerializeField] protected CurrentGameEvent GameEvent { get; set; }
        [field: SerializeField] protected PassStateEvent PassStateEvent { get; set; }
        [field: SerializeField] protected GameStats GameStats { get; set; }
        
        #endregion
        
        #region Properties
        
        /// <summary>
        /// Manager state required to save and load game. 
        /// </summary>
        public List<ISavableArgs> State { get; set; }
        
        #endregion
        
        #region Filds
        
        protected readonly System.Random systemRandom = new ();
        
        #endregion
        
        #region MonoBehaviour methods

        protected virtual void Start()
        {
            InitManager(InitItemMode.NewGame);
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

        /// <summary>
        /// Initializes manager at start or restart of game.
        /// </summary>
        protected abstract void InitManager(InitItemMode mode);

        // Event handler for CurrentGameEvent
        public abstract void OnEventRaised(ISubject<CurrentGameArgs> subject, CurrentGameArgs args);

        public virtual void Dispose()
        {
            GameEvent.Detach(this);
        }

        #endregion
    }
}