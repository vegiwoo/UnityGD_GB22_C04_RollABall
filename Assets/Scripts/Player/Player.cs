using System;
using GameDevLib.Args;
using GameDevLib.Events;
using GameDevLib.Helpers;
using GameDevLib.Interfaces;
using RollABall.Events;
using RollABall.Stats;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class Player : MonoBehaviour, IDisposable, GameDevLib.Interfaces.IObserver<InputManagerArgs>
    {
        #region Links
        
        [Header("Stats")]
        [SerializeField] protected PlayerStats playerStats;
        [field: SerializeField] protected GameStats gameStats;
        [Header("Events")]
        [SerializeField] private InputManagerEvent inputEvent;
        [SerializeField] protected PlayerEvent playerEvent;
        
        #endregion
        
        #region Fields
        
        protected Rigidbody playerRb;
        
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
        [field: SerializeField, ReadonlyField] protected float Velocity { get; set; }

        #endregion

        #region MonoBehaviour methods

        private void Awake()
        {
            playerRb = GetComponent<Rigidbody>();
        }

        protected virtual void Start()
        {
            GamePoints = 0;
            CurrentHp = playerStats.MaxHp;
            SpeedMultiplier = SpeedMultiplierConst;
        }
        
        protected virtual void Update()
        {
            SendNotify();
        }
        
        protected virtual void OnEnable()
        {
            inputEvent.Attach(this);
        }

        protected virtual void OnDisable()
        {
            inputEvent.Detach(this);
        }

        #endregion
        
        #region Functionality
        
        protected void Move()
        {
            if (!MoveDirection.HasValue) return;
            
            var value = MoveDirection.Value;
            var movement = new Vector3(value.x, 0, value.y);

            playerRb.AddForce(movement * SpeedMultiplier, ForceMode.Impulse);
        }
        
        public void OnEventRaised(ISubject<InputManagerArgs> subject, InputManagerArgs args)
        {
            MoveDirection = args.Moving;
        }

        /// <summary>
        /// Sends an event about changes in stats of a unit.
        /// </summary>
        /// <summary>
        /// Sends an event about changes in stats of a unit.
        /// </summary>
        protected abstract void SendNotify();

        public abstract void SetGamePoints(float points, bool increase);

        public abstract void SetHitPoints(float? hp, bool? increase, bool? isInvulnerable = null);

        public abstract void SetSpeed(float? multiplier, bool? increase, bool? cancelEffect = null);

        public virtual void Dispose()
        {
            inputEvent.Detach(this);
        }

        #endregion
    }
}