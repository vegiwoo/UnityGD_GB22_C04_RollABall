using GameDevLib.Interfaces;
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

        [field: Header("Other stats")] 
        [field: SerializeField] public EffectStats EffectStats { get; set; }
        
        [field: Header("Other events")] 
        [field: SerializeField] public EffectEvent EffectEvent { get; set; }
        [field: SerializeField] private ApplyEffectEvent ApplyEffectEvent { get; set; }
        
        [field: Header("Other repository")] 
        [field: SerializeField] public EffectRepository EffectRepository { get; set;}
        
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

        #endregion
        
        #region Funtionality

        public void OnEventRaised(ISubject<IEffectable> subject, IEffectable args)
        {
            ApplyEffectOnPlayer(args, args.Duration);
        }

        public override void Dispose()
        {
            base.Dispose();
            ApplyEffectEvent.Detach(this);
        }

        #endregion
    }
}