using System;
using System.Collections;
using System.Collections.Generic;
using GameDevLib.Interfaces;
using RollABall.Args;
using RollABall.Events;
using RollABall.Interactivity.Bonuses;
using RollABall.Interactivity.Effects;
using UnityEngine;
using static UnityEngine.Debug;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    public class EffectManager : MonoBehaviour, GameDevLib.Interfaces.IObserver<BonusArgs>, IDisposable
    {
        #region Properties
        
        [field: Header("Links")]
        [field: SerializeField] public Player.Player Player { get; set; }
        [field: SerializeField] public BonusEvent BonusEvent { get; set; }
        
        #endregion
        
        #region MonoBehaviour methods

        private void OnEnable()
        {
            BonusEvent.Attach(this);
        }

        private void OnDisable()
        {
            BonusEvent.Detach(this);
        }

        #endregion

        #region Funtionality
        
        public void Dispose()
        {
            BonusEvent.Detach(this);
        }
        
        public void OnEventRaised(ISubject<BonusArgs> subject, BonusArgs args)
        {
            if (args.Tag == GameData.PlayerTag)
            {
                ApplyEffectOnPlayer(args.Effect);
            }
        }

        private void ApplyEffectOnPlayer(IEffectable effect)
        {
            Log(effect.ToString());

            // Apply effects without duration
            if (effect.Duration is 0 && effect.BoosterType is BoosterType.None)
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
            else
            {
                // Apply effects with duration
                StartCoroutine(ApplyEffectCoroutine(effect));
            }

        }

        private IEnumerator ApplyEffectCoroutine(IEffectable effect)
        {
            if (effect.Duration is 0 && effect.BoosterType is BoosterType.None)
            {
                yield break;
            }

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
            
            switch (effect.EffectTarget)
            {
                case EffectTargetType.HitPoints:
                    if (effect.Type == EffectType.Buff)
                    {
                        Player.SetHitPoints(null, null, false);
                    }
                    break;
                case EffectTargetType.UnitSpeed:
                    Player.SetSpeed(null, null, true);
                    break;
                default:
                    yield break;
            }
            
            yield return null;
        }
        
        #endregion
    }
}