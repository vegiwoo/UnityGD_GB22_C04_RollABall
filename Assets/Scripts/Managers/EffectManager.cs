using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public class EffectManager : MonoBehaviour, IDisposable
    {
        #region Fields
        
        private List<IEffectable> _buffs;
        private List<IEffectable> _debuffs;
        
        private readonly Dictionary<EffectTargetType, Coroutine> _activeEffects = new ();

        #endregion
        
        #region Properties

        [field: Header("Links")] 
        [field: SerializeField] public EffectStats Stats { get; set; }
        [field: SerializeField] public Player.Player Player { get; set; }

        #endregion
        
        #region MonoBehaviour methods

        private void Start()
        {
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

        #endregion

        #region Funtionality
        
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
            switch (effect.EffectTarget)
            {
                case EffectTargetType.GamePoints:
                    Player.SetGamePoints(
                        effect.Type == EffectType.Buff ? 
                            effect.PositivePower : 
                            effect.NegativePower,
                        effect.Type == EffectType.Buff);

                    break;
                case EffectTargetType.HitPoints:

                    if (effect.Type == EffectType.Debuff)
                    {
                        Player.SetHitPoints(effect.NegativePower, false);
                    }
                    break;
            }
        }
        
        private IEnumerator ApplyEffectWithDurationCoroutine(IEffectable effect)
        {
            switch (effect.EffectTarget)
            {
                case EffectTargetType.HitPoints:
                    if (effect.Type == EffectType.Buff)
                    {
                        Player.SetHitPoints(null, null, true);
                    }
                    break;
                case EffectTargetType.UnitSpeed:
                    Player.SetSpeed(
                        effect.Type == EffectType.Buff ? effect.PositivePower : effect.NegativePower, 
                        effect.Type == EffectType.Buff);
                    break;
                default:
                    yield break;
            }

            // Apply effect
            yield return new WaitForSeconds(effect.Duration);

            StopEffectByType(effect.EffectTarget);
            
            yield return null;
        }

        private void StopEffectByType(EffectTargetType effectTargetType, EffectType? effectType = null)
        {
            if (!_activeEffects.ContainsKey(effectTargetType)) return;
            
            switch (effectTargetType)
            {
                case EffectTargetType.HitPoints:
                    if (effectType is EffectType.Buff)
                    {
                        Player.SetHitPoints(null, null, false);
                    }
                    break;
                case EffectTargetType.UnitSpeed:
                    Player.SetSpeed(null, null, true);
                    break;
            }

            StopCoroutine(_activeEffects[effectTargetType]);
            _activeEffects.Remove(effectTargetType);
            
            Log($"Stop active effect on target {effectTargetType}");
        }
  
        public void Dispose()
        {
            _buffs = _debuffs = null;
        }
        
        #endregion
    }
}