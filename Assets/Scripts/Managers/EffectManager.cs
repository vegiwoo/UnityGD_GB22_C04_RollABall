using System.Collections;
using System.Collections.Generic;
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
    public class EffectManager : BaseManager, IObserver<IEffectable>
    {
        #region Fields
        
        private readonly Dictionary<EffectTargetType, Coroutine> _activeEffects = new ();
        
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
        }

        public void ApplyEffectOnPlayer(IEffectable effect)
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
                var effectCoroutine = StartCoroutine(ApplyEffectWithDurationCoroutine(effect));
                // Store effect Coroutine in dictionary
                _activeEffects[effect.EffectTarget] = effectCoroutine;
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
        
        private IEnumerator ApplyEffectWithDurationCoroutine(IEffectable effect)
        {
            var args = new EffectArgs(effect.Type, effect.EffectTarget);
            
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
            yield return new WaitForSeconds(effect.Duration);

            StopEffectByType(effect.EffectTarget, effect.Type);
            
            yield return null;
        }

        private void StopEffectByType(EffectTargetType effectTargetType, EffectType? effectType = null)
        {
            if (!_activeEffects.ContainsKey(effectTargetType)) return;

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

            StopCoroutine(_activeEffects[effectTargetType]);
            _activeEffects.Remove(effectTargetType);
            
            Log($"Stop active effect on target {effectTargetType}");
        }

        // Event handler for CurrentGameEvent
        public override void OnEventRaised(ISubject<CurrentGameArgs> subject, CurrentGameArgs args)
        {
            if (args.IsRestartGame)
            {
                InitManager();
                
                // Send Rebirth Effect
                EffectEvent.Notify(new EffectArgs(EffectType.Buff, EffectTargetType.Rebirth));
            }
        }

        public void OnEventRaised(ISubject<IEffectable> subject, IEffectable args)
        {
            ApplyEffectOnPlayer(args);
        }
        
        #endregion
    }
}