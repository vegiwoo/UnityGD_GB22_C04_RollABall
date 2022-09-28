using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameDevLib.Interfaces;
using RollABall.Args;
using RollABall.Events;
using RollABall.Interactivity.Bonuses;
using RollABall.Interactivity.Effects;
using RollABall.Stats;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Debug;
using Random = UnityEngine.Random;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    public class EffectManager : BaseManager
    {
        #region Fields
        
        private List<IEffectable> _buffs;
        private List<IEffectable> _debuffs;
        
        private readonly Dictionary<EffectTargetType, Coroutine> _activeEffects = new ();

        #endregion
        
        #region Properties

        [field: Header("Links")] 
        [field: SerializeField] public EffectStats Stats { get; set; }
        [field: SerializeField] public EffectEvent EffectEvent { get; set; }

        #endregion
        
        #region MonoBehaviour methods

        private void Start()
        {
            InitManager();
        }

        #endregion

        #region Funtionality
        
        private void InitManager()
        {
            StopAllCoroutines();
            
            // Thrown Exception Implementation
            if (Stats == null)
            {
                throw new ArgumentNullException(Stats.effects.ToString());
            }

            try
            {
                _buffs = Stats.effects
                    .Select(el => el as IEffectable)
                    .Where(el => el.Type == EffectType.Buff)
                    .ToList();

                _debuffs = Stats.effects
                    .Select(el => el as IEffectable)
                    .Where(el => el.Type == EffectType.Debuff)
                    .ToList();
            }
            catch (ArgumentNullException e)
            {
                LogError("Link to effect stats cannot be empty");
                EditorApplication.isPlaying = false;
            }
        }
        
        /// <summary>
        /// Finds a random EffectFactoryKey according to EffectType, a factory and generates an effect.
        /// </summary>
        /// <param name="effectType">Type of effect to generate.</param>
        /// <returns>Random effect.</returns>
        public IEffectable GetRandomEffectByType(EffectType effectType)
        {
            return effectType switch
            {
                EffectType.Buff => _buffs[Random.Range(0, _buffs.Count)],
                EffectType.Debuff => _debuffs[Random.Range(0, _debuffs.Count)],
                _ => default
            };
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

        #endregion
    }
}