using System;
using GameDevLib.Args;
using GameDevLib.Events;
using GameDevLib.Helpers;
using GameDevLib.Interfaces;
using RollABall.Args;
using RollABall.Events;
using RollABall.Interactivity.Effects;
using RollABall.Stats;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class Player : MonoBehaviour, IDisposable, GameDevLib.Interfaces.IObserver<InputManagerArgs>, GameDevLib.Interfaces.IObserver<EffectArgs>
    {
        #region Links
        
        [Header("Stats")]
        [SerializeField] protected PlayerStats playerStats;
        [field: SerializeField] protected GameStats gameStats;
        [Header("Events")]
        [SerializeField] private InputManagerEvent inputEvent;
        [SerializeField] private EffectEvent effectEvent;
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
        
        protected virtual void OnEnable()
        {
            inputEvent.Attach(this);
            effectEvent.Attach(this);
        }

        protected virtual void OnDisable()
        {
            Dispose();
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
            inputEvent.Detach(this);
            effectEvent.Detach(this);
        }

        #endregion
    }
}