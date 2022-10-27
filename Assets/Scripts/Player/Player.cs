using System;
using System.Collections.Generic;
using GameDevLib.Args;
using GameDevLib.Events;
using GameDevLib.Helpers;
using GameDevLib.Interfaces;
using RollABall.Args;
using RollABall.Events;
using RollABall.Infrastructure;
using RollABall.Interactivity.Effects;
using RollABall.Stats;
using UnityEngine;
using EffectArgs = RollABall.Args.EffectArgs;
using EffectEvent = RollABall.Events.EffectEvent;

// ReSharper disable once CheckNamespace
namespace RollABall.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract partial class Player : MonoBehaviour, IActionable,
        GameDevLib.Interfaces.IObserver<InputManagerArgs>, GameDevLib.Interfaces.IObserver<EffectArgs>, IDisposable
    {
        #region Links

        [field:Header("Main stats")] 
        [field:SerializeField] protected PlayerStats PlayerStats { get; set; }
        [field: SerializeField] protected GameStats GameStats { get; set; }

        [field:Header("Main events")] 
        [field:SerializeField] private InputEvent InputEvent { get; set; }
        [field:SerializeField] private EffectEvent EffectEvent { get; set; }
        [field:SerializeField] protected PlayerEvent PlayerEvent { get; set; }
        [field:SerializeField] protected CurrentGameEvent GameEvent{ get; set; }

        #endregion

        #region Fields

        protected Rigidbody playerRb;

        #endregion

        #region Properties

        // Game 
        [field: SerializeField, ReadonlyField] protected float GamePoints { get; set; }

        // HP
        [field: SerializeField, ReadonlyField] protected float CurrentHp { get; set; }
        [field: SerializeField, ReadonlyField] protected bool IsUnitInvulnerable { get; set; }

        // Movement
        private Vector2? MoveDirection { get; set; } = null;
        protected const float SpeedMultiplierConst = 3f;
        [field: SerializeField, ReadonlyField] protected float SpeedMultiplier { get; set; }

        public abstract List<ISavableArgs> SavedState { get; set; }

        #endregion

        #region MonoBehaviour methods

        private void Awake()
        {
            playerRb = GetComponent<Rigidbody>();
        }
        
        protected virtual void OnEnable()
        {
            InputEvent.Attach(this);
            EffectEvent.Attach(this);
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
        
        protected void Move()
        {
            if (!MoveDirection.HasValue) return;

            var value = MoveDirection.Value;
            var movement = new Vector3(value.x, 0, value.y);

            playerRb.AddForce(movement * SpeedMultiplier, ForceMode.Impulse);
        }

        // Event handler for InputManagerEvent
        public void OnEventRaised(ISubject<InputManagerArgs> subject, InputManagerArgs args)
        {
            MoveDirection = args.Moving;
        }

        // Event handler for EffectEvent
        public void OnEventRaised(ISubject<EffectArgs> subject, EffectArgs args)
        {
            switch (args.EffectTargetType)
            {
                case EffectTargetType.GamePoints:
                    SetGamePoints(args.Power, args.Increase);
                    break;
                case EffectTargetType.HitPoints:
                    SetHitPoints(args.Power, args.Increase, args.IsInvulnerable);
                    break;
                case EffectTargetType.UnitSpeed:
                    SetSpeed(args.Power, args.Increase, args.CancelEffect);
                    break;
                case EffectTargetType.Rebirth:
                    //transform.position = gameStats.PlayerSpawnPoint;
                    //InitPlayer();
                    break;
            }

            SendNotify();
        }

        /// <summary>
        /// Sends an event about changes in stats of a unit.
        /// </summary>
        /// <summary>
        /// Sends an event about changes in stats of a unit.
        /// </summary>
        protected abstract void SendNotify();

        protected abstract void SetGamePoints(float? points, bool? increase);

        protected abstract void SetHitPoints(float? hp, bool? increase, bool? isInvulnerable = null);

        protected abstract void SetSpeed(float? multiplier, bool? increase, bool? cancelEffect = null);

        public virtual void Dispose()
        {
            InputEvent.Detach(this);
            EffectEvent.Detach(this);
        }

        #endregion
    }

}