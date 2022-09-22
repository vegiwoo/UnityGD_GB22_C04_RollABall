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
        // Game 
        [field: SerializeField, ReadonlyField] protected float GamePoints { get; set; }
        
        // HP
        [field: SerializeField, ReadonlyField] protected float CurrentHp { get; set; }
        [field:SerializeField, ReadonlyField] protected bool IsUnitInvulnerable { get; set; }
        
        // Movement
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
            SpeedMultiplier = SpeedMultiplierConst;
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
            if (!MoveDirection.HasValue) return;
            
            var value = MoveDirection.Value;
            var movement = new Vector3(value.x, 0, value.y);

            _rigidbody.AddForce(movement * SpeedMultiplier, ForceMode.Impulse);
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
        
        public void SetHitPoints(float? hp, bool? increase, bool? isInvulnerable = null)
        {
            // Set Invulnerable
            if (isInvulnerable.HasValue)
            {
                IsUnitInvulnerable = isInvulnerable.Value;
            }

            // Set HP (only !increase)
            if (hp.HasValue && increase.HasValue && !increase.Value && !IsUnitInvulnerable)
            {
                CurrentHp = CurrentHp - hp.Value > 0
                    ? CurrentHp -= hp.Value
                    : 0;
            }
        }

        public void SetSpeed(float? multiplier, bool? increase, bool? cancelEffect = null)
        {
            if (cancelEffect.HasValue && cancelEffect.Value)
            {
                SpeedMultiplier = SpeedMultiplierConst;
                return;
            }

            if (multiplier.HasValue && increase.HasValue)
            {
                switch (increase.Value)
                {
                    case true:
                        if (SpeedMultiplier > SpeedMultiplierConst)
                        {
                            return;
                        }
                        else
                        {
                            SpeedMultiplier = SpeedMultiplierConst * multiplier.Value;
                        }

                        break;
                    case false:
                        if (SpeedMultiplier < SpeedMultiplierConst)
                        {
                            return;
                        }
                        else
                        {
                            SpeedMultiplier = SpeedMultiplierConst / multiplier.Value;
                        }
                        break;
                }
            }
            
            
            
            
            // Если тповыещение и уже повышкено - ничего не делать


            // Если понимнижение и уже понижено - ничего не далеть
            
            
            
            // Если повышение и текущий SpeedMultiplier == SpeedMultiplierConst
            // SpeedMultiplier = SpeedMultiplierConst * multiplier
            
            // Если   и текущий SpeedMultiplier == SpeedMultiplierConst
            
            
            
            switch (increase)
            {
                case true:
                    
                    break;
                case false:
                    break;
            }
        }

        public virtual void Dispose()
        {
            inputEvent.Detach(this);
        }

        #endregion
    }
}