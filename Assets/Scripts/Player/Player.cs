using GameDevLib.Args;
using GameDevLib.Events;
using GameDevLib.Interfaces;
using RollABall.Args;
using RollABall.Events;
using RollABall.Stats;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class Player : MonoBehaviour, IObserver<InputManagerArgs>
    {
        #region Links
        [Header("Stats")]
        [SerializeField] private PlayerStats playerStats;
        [Header("Events")]
        [SerializeField] private InputManagerEvent inputEvent;
        [SerializeField] protected BonusEvent bonusEvent;
        [SerializeField] protected PlayerEvent playerEvent;
        #endregion
        
        #region Constants and variables
        private Rigidbody _rigidbody;
        #endregion
        
        #region Properties
        
        protected float CurrentHp { get; set; }
        private float CurrentSpeed { get; set; }

        private Vector2? MoveDirection { get; set; } = null;
        
        #endregion

        #region MonoBehaviour methods

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        protected virtual void Start()
        {
            CurrentHp = playerStats.MaxHp;
            CurrentSpeed = playerStats.MaxHp;

            var args = new PlayerArgs(CurrentHp);
            playerEvent.Notify(args);
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
            _rigidbody.AddForce(movement * CurrentSpeed * Time.fixedDeltaTime, ForceMode.Impulse);
        }
        
        public void OnEventRaised(ISubject<InputManagerArgs> subject, InputManagerArgs args)
        {
            MoveDirection = args.Moving;
        }

        #endregion
    }
}