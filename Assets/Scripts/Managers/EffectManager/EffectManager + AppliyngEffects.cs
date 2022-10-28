using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using RollABall.Args;
using RollABall.Interactivity.Bonuses;
using RollABall.Interactivity.Effects;
using UnityEngine;
using static UnityEngine.Debug;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    public partial class EffectManager
    {
        #region Fields
        
        private List<ActiveEffectArg> _activeEffectsOnPlayer;
        
        #endregion
        
        #region Functionality
        private async void ApplyEffectOnPlayer(IEffectable effect, float remainingDuration = 0)
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
                    
                // Apply effects with duration and store in dict
                var task = ApplyEffectWithDurationTask(effect, new CancellationTokenSource(), remainingDuration);
                await task;
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

        private async UniTask ApplyEffectWithDurationTask(IEffectable effect, CancellationTokenSource cancellationSource, 
            float remainingDuration = 0)
        {
            var activeEffectArg = new ActiveEffectArg(effect, cancellationSource, remainingDuration);
            _activeEffectsOnPlayer.Add(activeEffectArg);
            
            var effectArgs = new EffectArgs(effect.Type, effect.EffectTarget);
            
            switch (effect.EffectTarget)
            {
                case EffectTargetType.HitPoints:
                    if (effect.Type == EffectType.Buff)
                    {
                        effectArgs.Init(null,null, true);
                    }
                    break;
                case EffectTargetType.UnitSpeed:
                    effectArgs.Init(effect.Type == EffectType.Buff ? 
                            effect.PositivePower : 
                            effect.NegativePower, 
                        effect.Type == EffectType.Buff);
                    break;
            }
            
            // Apply effect notify
            EffectEvent.Notify(effectArgs);

            // Apply effect
            var timer = remainingDuration > 0 ? remainingDuration : effect.Duration;
            var step = 1;
            
            do
            {
                try
                {
                    activeEffectArg.RemainingDuration = timer;
                }
                catch (Exception e)
                {
                    Log(e.Message);
                }
                
                timer -= step;
                await UniTask.Delay(TimeSpan.FromSeconds(step));

            } while (timer > 0);
            
            await UniTask.WaitUntil(() => timer <= 0, cancellationToken: cancellationSource.Token);
 
            // Stop effect 
            StopEffectByType(effect.EffectTarget, effect.Type);
            
            // // breaks further execution of this method
            // cancellationSource.Token.ThrowIfCancellationRequested();
        }
        
        
        private IEnumerator ApplyEffectWithDurationCoroutine(IEffectable effect, float remainingDuration = 0)
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
                var timer = remainingDuration > 0 ? remainingDuration : effect.Duration;
                while (timer > 0)
                {
                   // _activeEffectsOnPlayer[effect].RemainingDuration = timer;
                    timer -= Time.deltaTime;
                    yield return null;
                }
 
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
            var activeEffectByType =
                _activeEffectsOnPlayer
                    .SingleOrDefault(el => el.Effect.EffectTarget == effectTargetType);

            if (activeEffectByType is not null)
            {
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
                //StopCoroutine(_activeEffects[activeEffectByType.Key].Routine);
                activeEffectByType.Dispose();
                if (_activeEffectsOnPlayer.Contains(activeEffectByType))
                {
                    _activeEffectsOnPlayer.Remove(activeEffectByType);
                }
                
                Log($"Stop active effect on target {effectTargetType}");
            }
        }
        
        #endregion
    }
}