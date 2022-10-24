using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameDevLib.Interfaces;
using RollABall.Args;
using RollABall.Events;
using RollABall.Interactivity.Bonuses;
using RollABall.Interactivity.Effects;
using RollABall.ScriptableObjects;
using RollABall.Stats;
using UnityEngine;
using static UnityEngine.Debug;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    public partial class EffectManager : BaseManager, GameDevLib.Interfaces.IObserver<IEffectable>
    {
        #region Fields
        
        private readonly Dictionary<EffectTargetType, Coroutine> _activeEffectsByTarget = new ();
        
        #endregion
        
        #region Properties

        [field: Header("Links")] 
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
            
            // Pattern 'Memento' - init caretaker
            Caretaker.Init(this, "Effects", "EffectsMemento");
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            ApplyEffectEvent.Detach(this);
        }

        #endregion
        
        #region Funtionality
        
        protected override void InitManager(bool fromLoad = false)
        {
            StopAllCoroutines();
            _activeEffectsByTarget.Clear();
            
            if (fromLoad)
            {
                foreach (var item in State)
                {
                    ApplyEffectOnPlayer(item.AppliedEffect, item.RemainingDuration);
                }
            }
            else
            {
                State.Clear();
            }

        }
        
        private void ApplyEffectOnPlayer(IEffectable effect, float remainingDuration = 0)
        {
            Log(effect.ToString());

            // Apply effects without duration
            if (effect.Duration is 0 && effect.BoosterType is BoosterType.None)
            {
                ApplyEffectWithoutDuration(effect);
            }
            else
            {
                // Stopping active effect with duration on same target
                StopEffectByType(effect.EffectTarget); 
                    
                // Apply effects with duration
                var effectCoroutine = StartCoroutine(ApplyEffectWithDurationCoroutine(effect, remainingDuration));
                // Store effect Coroutine in dictionary
                _activeEffectsByTarget[effect.EffectTarget] = effectCoroutine;
            }
        }

        private void ApplyEffectWithoutDuration(IEffectable effect)
        {
            var args = new EffectArgs(effect.Type, effect.EffectTarget);
            
            switch (effect.EffectTarget)
            {
                case EffectTargetType.GamePoints:
                    args.Init(effect.Type == EffectType.Buff ? 
                        effect.PositivePower : 
                        effect.NegativePower, effect.Type == EffectType.Buff);
                    break;
                case EffectTargetType.HitPoints:

                    if (effect.Type == EffectType.Debuff)
                    {
                        args.Init(effect.NegativePower, false);
                    }
                    break;
            }
            
            EffectEvent.Notify(args);
        }
        
        private IEnumerator ApplyEffectWithDurationCoroutine(IEffectable effect, float remainingDuration = 0)
        {
            var args = new EffectArgs(effect.Type, effect.EffectTarget);
            
            // Add new effect in manager state.
            var effectStateArgs = new EffectSaveArgs(effect as Effect, effect.Duration);
            State.Add(effectStateArgs);
            
            switch (effect.EffectTarget)
            {
                case EffectTargetType.HitPoints:
                    if (effect.Type == EffectType.Buff)
                    {
                        args.Init(null,null, true);
                    }
                    break;
                case EffectTargetType.UnitSpeed:
                    args.Init(effect.Type == EffectType.Buff ? 
                        effect.PositivePower : 
                        effect.NegativePower, 
                        effect.Type == EffectType.Buff);
                    break;
                default:
                    yield break;
            }
            
            // Apply effect notify
            EffectEvent.Notify(args);

            // Apply effect
            var timer = remainingDuration > 0 ? remainingDuration : effect.Duration;
            while (timer > 0)
            {
                var effectInState = State
                    .SingleOrDefault(el => el.AppliedEffect == effect);
                
                if (effectInState != null)
                {
                    effectInState.RemainingDuration = timer;
                }
                else
                {
                    LogError($"Effect not found in effect manager state (by target {args.EffectTargetType})");
                }

                timer -= Time.deltaTime;
                yield return null;
            }
      
            //yield return new WaitForSeconds(effect.Duration);

            // Stop effect 
            StopEffectByType(effect.EffectTarget, effect.Type);

            yield return null;
        }

        /// <summary>
        /// Stops an effect by its type.
        /// </summary>
        /// <param name="effectTargetType">Type of effect target.</param>
        /// <param name="effectType">Type of effect.</param>
        /// <remarks>Applied when receiving an effect again on same target as applied one.</remarks>>
        private void StopEffectByType(EffectTargetType effectTargetType, EffectType? effectType = null)
        {
            if (!_activeEffectsByTarget.ContainsKey(effectTargetType)) return;

            EffectArgs args = default;
            
            if (effectType.HasValue)
            {
                args = new EffectArgs(effectType.Value, effectTargetType);
                
                switch (effectTargetType)
                {
                    case EffectTargetType.HitPoints:
                        if (effectType is EffectType.Buff)
                        {
                            args.Init(null, null, false);
                        }
                        break;
                    case EffectTargetType.UnitSpeed:
                        args.Init(null, null, null, true);
                        break;
                }
            }
            
            if(args is not null)
            {
                EffectEvent.Notify(args);
            }

            // Stop coroutines with effects on the target and give from the dictionary
            StopCoroutine(_activeEffectsByTarget[effectTargetType]);
            _activeEffectsByTarget.Remove(effectTargetType);

            // Removing effects on target from state of manager.
           //State.RemoveAll(el => el.AppliedEffect.EffectTarget == effectTargetType);
            
            Log($"Stop active effect on target {effectTargetType}");
        }

        // Event handler for CurrentGameEvent
        public override void OnEventRaised(ISubject<CurrentGameArgs> subject, CurrentGameArgs args)
        {
            if (args.IsRestartGame)
            {
                InitManager();
                
                // Send Rebirth Effect
                // EffectEvent.Notify(new EffectArgs(EffectType.Buff, EffectTargetType.Rebirth));
            }
            
            
            if (args.IsSaveGame)
            {
                try
                {
                    Caretaker.Save();
                }
                catch (Exception e)
                {
                    LogException(e);
                }
            }

            if (args.IsLoadGame)
            {
                try
                {
                    Caretaker.Load();
                }
                catch (Exception e)
                {
                    LogException(e);
                }
            }
   
        }

        public void OnEventRaised(ISubject<IEffectable> subject, IEffectable args)
        {
            ApplyEffectOnPlayer(args);
        }
        
        #endregion
    }
}