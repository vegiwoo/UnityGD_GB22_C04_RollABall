using System;
using System.Collections.Generic;
using GameDevLib.Args;
using GameDevLib.Events;
using GameDevLib.Helpers;
using GameDevLib.Interfaces;
using RollABall.Args;
using RollABall.Events;
using RollABall.Interactivity.Effects;
using RollABall.Managers;
using RollABall.ScriptableObjects;
using RollABall.Stats;
using UnityEngine;
using UnityEngine.Serialization;
using EffectArgs = RollABall.Args.EffectArgs;
using EffectEvent = RollABall.Events.EffectEvent;
using static UnityEngine.Debug;

// ReSharper disable once CheckNamespace
namespace RollABall.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class Player : MonoBehaviour, IDisposable, GameDevLib.Interfaces.IObserver<InputManagerArgs>, 
        GameDevLib.Interfaces.IObserver<CurrentGameArgs>,  GameDevLib.Interfaces.IObserver<EffectArgs>
    {
        #region Links
        
        [Header("Stats")]
        [SerializeField] protected PlayerStats playerStats;
        [field: SerializeField] protected GameStats gameStats;
        
        [Header("Events")]
        [SerializeField] private InputEvent inputEvent;
        [SerializeField] private EffectEvent effectEvent;
        [SerializeField] protected PlayerEvent playerEvent;
        [SerializeField] protected CurrentGameEvent gameEvent;
        [SerializeField] protected PassStateEvent passStateEvent;

        #endregion
        
        #region Fields

        private Rigidbody _playerRb;
        
        #endregion

        #region Properties
        // Game 
        [field: SerializeField, ReadonlyField] protected float GamePoints { get; set; }
        
        // HP
        [field: SerializeField, ReadonlyField] protected float CurrentHp { get; set; }
        [field:SerializeField, ReadonlyField] protected bool IsUnitInvulnerable { get; set; }
        
        // Movement
        private Vector2? MoveDirection { get; set; } = null;
        protected const float SpeedMultiplierConst = 3f;
        [field: SerializeField, ReadonlyField] protected float SpeedMultiplier { get; set; }
        
        // Memento - State 
        public PlayerArgs State { get; set; }

        #endregion

        #region MonoBehaviour methods

        private void Awake()
        {
            _playerRb = GetComponent<Rigidbody>();
        }

        protected virtual void Start()
        {
            InitPlayer(InitItemMode.NewGame);
        }
        
        protected virtual void OnEnable()
        {
            gameEvent.Attach(this);
            inputEvent.Attach(this);
            effectEvent.Attach(this);
        }

        protected virtual void OnDisable()
        {
            Dispose();
        }

        #endregion
        
        #region Functionality
        
        protected void InitPlayer(InitItemMode mode)
        {
            switch (mode)
            {
                case InitItemMode.LoadGame:

                    GamePoints = State.GamePoints;
                    CurrentHp = State.CurrentHp;
                    IsUnitInvulnerable = State.IsUnitInvulnerable;
                    SpeedMultiplier = SpeedMultiplierConst;

                    transform.position = new Vector3(State.Point.PosX, State.Point.PosY, State.Point.PosZ);
                    
                    break;
                default:

                    GamePoints = 0;
                    CurrentHp = playerStats.MaxHp;
                    IsUnitInvulnerable = false;
                    SpeedMultiplier = SpeedMultiplierConst;
                    
                    transform.position = gameStats.PlayerSpawnPoint;

                    break;
            }
            
            // Stop physical move on reboot
            _playerRb.velocity = new Vector3(0, _playerRb.velocity.y, 0);
        }
        
        protected void Move()
        {
            if (!MoveDirection.HasValue) return;
            
            var value = MoveDirection.Value;
            var movement = new Vector3(value.x, 0, value.y);

            _playerRb.AddForce(movement * SpeedMultiplier, ForceMode.Impulse);
        }

        public abstract PlayerArgs MakeState();
        
        // Event handlers
        public void OnEventRaised(ISubject<InputManagerArgs> subject, InputManagerArgs args)
        {
            MoveDirection = args.Moving;
        }
        
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
                    // ... 
                    break;
            }
            
           SendNotify();
        }
        
        public void OnEventRaised(ISubject<CurrentGameArgs> subject, CurrentGameArgs args)
        { 
            if (args.IsRestartGame)
            {
                InitPlayer(InitItemMode.RestartGame);
                SendNotify();
            }
            
            if (args.IsSaveGame)
            {
                State = MakeState();
                passStateEvent.Notify(new List<ISavableArgs>(1){State});
            }
            
            // Load game
            if (args.SaveGameArgs != null)
            {
                State = args.SaveGameArgs.PlayerArgs;
                InitPlayer(InitItemMode.LoadGame);
            }
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
            gameEvent.Detach(this);
            inputEvent.Detach(this);
            effectEvent.Detach(this);
        }

        #endregion
    }
}