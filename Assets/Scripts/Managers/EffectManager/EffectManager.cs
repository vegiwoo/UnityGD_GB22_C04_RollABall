using GameDevLib.Interfaces;
using RollABall.Args;
using RollABall.Events;
using RollABall.Interactivity.Bonuses;
using RollABall.ScriptableObjects;
using RollABall.Stats;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    public partial class EffectManager : BaseManager, IObserver<IEffectable>
    {
        #region Links

        [field: Header("Links")] 
        [field: SerializeField] public EffectStats EffectStats { get; set; }
        [field: SerializeField] public EffectRepository EffectRepository { get; set;}
        [field: SerializeField] public EffectEvent EffectEvent { get; set; }
        [field: SerializeField] private ApplyEffectEvent ApplyEffectEvent { get; set; }

        [field: Header("Memento")] 
        [field: SerializeField] private SaveGameEvent SaveGameEvent { get; set; }
        #endregion
        
        #region Monobehavior metods

        protected override void OnEnable()
        {
            base.OnEnable();
            
            EffectRepository.Init(EffectStats);
            
            ApplyEffectEvent.Attach(this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            ApplyEffectEvent.Detach(this);
        }

        #endregion
        
        #region Funtionality

        // Event handler for CurrentGameEvent
        public override void OnEventRaised(ISubject<CurrentGameArgs> subject, CurrentGameArgs args)
        {
         
        }

        public void OnEventRaised(ISubject<IEffectable> subject, IEffectable args)
        {
            ApplyEffectOnPlayer(args);
        }
        
        #endregion
    }
}