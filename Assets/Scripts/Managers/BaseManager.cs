using System;
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

        [field: SerializeField] protected CurrentGameEvent GameEvent { get; set; }
        [field: SerializeField] protected GameStats GameStats { get; set; }
        
        #endregion
        
        #region Filds
        
        protected readonly System.Random systemRandom = new ();
        
        #endregion
        
        #region MonoBehaviour methods

        protected void Start()
        {
            InitManager();
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
        protected abstract void InitManager();
        
        // Event handler for CurrentGameEvent
        public abstract void OnEventRaised(ISubject<CurrentGameArgs> subject, CurrentGameArgs args);

        public virtual void Dispose()
        {
            GameEvent.Detach(this);
        }

        #endregion
    }
}