using System;
using GameDevLib.Args;
using GameDevLib.Events;
using GameDevLib.Helpers;
using GameDevLib.Interfaces;
using RollABall.Args;
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
        
        #region Constants and variables
        
        private Rigidbody _rigidbody;
        #endregion

        #region Properties
        
        [field: SerializeField, ReadonlyField] protected float GamePoints { get; set; }
        [field: SerializeField, ReadonlyField] protected float CurrentHp { get; set; }
        [field:SerializeField, ReadonlyField] protected bool IsUnitInvulnerable { get; set; }
        
        private Vector2? MoveDirection { get; set; } = null;

        protected const float SpeedMultiplierConst = 3f;
        [field: SerializeField, ReadonlyField] protected float SpeedMultiplier { get; set; }

        [field: SerializeField, ReadonlyField] private float Velocity { get; set; }

        #endregion

        #region MonoBehaviour methods

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        protected virtual void Start()
        {
            GamePoints = 0;
            CurrentHp = playerStats.MaxHp;
            SpeedMultiplier = SpeedMultiplier;
        }
        
        protected virtual void Update()
        {
            Velocity = _rigidbody.velocity.magnitude;
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
            if (!MoveDirection.HasValue || Velocity >= playerStats.MaxSpeed) return;
            
            var value = MoveDirection.Value;
            var movement = new Vector3(value.x, 0, value.y);

            _rigidbody.AddForce(movement * SpeedMultiplier, ForceMode.VelocityChange);
        }
        
        public void OnEventRaised(ISubject<InputManagerArgs> subject, InputManagerArgs args)
        {
            MoveDirection = args.Moving;
        }

        /// <summary>
        /// Sends an event about changes in stats of a unit.
        /// </summary>
        private void SendNotify()
        {
            var isSpeedUp = SpeedMultiplier > SpeedMultiplierConst;
            var isSpeedDown = SpeedMultiplier < SpeedMultiplierConst;

            var args = new PlayerArgs(
                CurrentHp, 
                IsUnitInvulnerable, 
                Velocity, 
                isSpeedUp, 
                isSpeedDown,
                (int)GamePoints
                );
            
            playerEvent.Notify(args);
        }

        public void SetGamePoints(float points, bool increase)
        {
            GamePoints = increase switch
            {
                true => GamePoints + points >= gameStats.GameHighScore
                    ? gameStats.GameHighScore
                    : GamePoints += points,
                false => GamePoints - points > 0 
                    ? GamePoints -= points 
                    : 0
            };
        }
        
        public void SetHitPoints(float hp, bool increase, bool isInvulnerable)
        {
            IsUnitInvulnerable = isInvulnerable;
            
            CurrentHp = increase switch
            {
                true => CurrentHp, 
                false => CurrentHp - hp > 0 && !IsUnitInvulnerable
                    ? CurrentHp -= hp 
                    : CurrentHp
            };
        }
        
        public void SetSpeed(float multiplier, bool increase)
        {
            
        }

        public virtual void Dispose()
        {
            inputEvent.Detach(this);
        }

        #endregion
    }
}