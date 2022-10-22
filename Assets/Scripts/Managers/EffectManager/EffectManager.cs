using System;
using System.Collections.Generic;
using System.Linq;
using GameDevLib.Interfaces;
using RollABall.Args;
using RollABall.Events;
using RollABall.Interactivity.Bonuses;
using RollABall.ScriptableObjects;
using RollABall.Stats;
using UnityEngine;
using static UnityEngine.Debug;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    public partial class EffectManager : BaseManager, GameDevLib.Interfaces.IObserver<IEffectable>
    {
        #region Properties

        [field: Header("Other links")] 
        [field: SerializeField] public EffectStats Stats { get; set; }
        [field: SerializeField] public EffectRepository EffectRepository { get; set;}
        [field: SerializeField] public EffectEvent EffectEvent { get; set; }
        [field: SerializeField] private ApplyEffectEvent ApplyEffectEvent { get; set; }

        #endregion
        
        #region Monobehavior metods

        protected override void OnEnable()
        {
            base.OnEnable();
            
            EffectRepository.Init(Stats);
            
            ApplyEffectEvent.Attach(this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            ApplyEffectEvent.Detach(this);
        }

        #endregion
        
        #region Funtionality
        
        protected override void InitManager(InitItemMode mode)
        {
            StopAllCoroutines();
            
            _activeEffectsByTarget.Clear();

            switch (mode)
            {
                case InitItemMode.LoadGame:
                    
                    foreach (var item in State.Cast<EffectSaveArgs>().ToList())
                    {
                        ApplyEffectOnPlayer(item.AppliedEffect, item.RemainingDuration);
                    }
                    
                    break;
                default:
                    
                    // Manager state initialization.
                    State = new List<ISavableArgs>(new List<EffectSaveArgs>());
                    
                    break;
            }
        }
        
        // Event handler for CurrentGameEvent
        public override void OnEventRaised(ISubject<CurrentGameArgs> subject, CurrentGameArgs args)
        {
            // Restart game
            if (args.IsRestartGame)
            {
                InitManager(InitItemMode.RestartGame);
            }
            
            // Save game 
            if (args.IsSaveGame)
            {
                PassStateEvent.Notify(State);
            }

            // Load game
            if (args.SaveGameArgs != null)
            {
                State = new List<ISavableArgs>(args.SaveGameArgs.EffectSaveArgs);
                InitManager(InitItemMode.LoadGame);
            }
   
        }

        public void OnEventRaised(ISubject<IEffectable> subject, IEffectable args)
        {
            ApplyEffectOnPlayer(args);
        }

        #endregion
    }
}