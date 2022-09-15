using GameDevLib.Args;
using GameDevLib.Events;
using GameDevLib.Interfaces;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class Player : MonoBehaviour, GameDevLib.Interfaces.IObserver<InputManagerArgs>
    {
        #region Links

        [SerializeField] private InputManagerEvent inputEvent;

        private Rigidbody _rigidbody;
        #endregion
        
        #region Properties
        
        [field: SerializeField, Range(1f, 5f)]
        private float Speed { get; set; } = 5.0f;

        private Vector2? MoveDirection { get; set; } = null;
        
        #endregion

        #region MonoBehaviour methods

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            inputEvent.Attach(this);
        }

        private void OnDisable()
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
            _rigidbody.AddForce(movement * Speed, ForceMode.Impulse);
        }
        
        public void OnEventRaised(ISubject<InputManagerArgs> subject, InputManagerArgs args)
        {
            MoveDirection = args.Moving;
        }
        
        #endregion
    }
}