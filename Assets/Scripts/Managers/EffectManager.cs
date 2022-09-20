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
                ApplyEffect(args.Effect);
            }
        }

        private void ApplyEffect(IEffectable effect)
        {
            Log(effect.ToString());

            // Apply effects without duration
            if (effect.Duration == 0 && effect.BoosterType == BoosterType.None)
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
                            Player.SetHitPoints(effect.NegativePower, false, false);
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
            // ... HP buff 
            // ... Speed up Speed Down
            
            yield return null;
        }
        
        #endregion
    }
}

      // // TODO: Applying effects -> to separate manager
        // private void ApplyEffect(IEffectable effect)
        // {
        //     Log(effect.ToString());
        //
        //    
        //     if (effect.Duration == 0 && effect.BoosterType == BoosterType.None)
        //     {
        //         switch (effect.EffectTarget)
        //         {
        //             case EffectTargetType.GamePoints:
        //                 switch (effect.Type)
        //                 {
        //                     case EffectType.Buff:
        //                         CurrentScore = CurrentScore + effect.PositivePower < gameStats.GameHighScore ? 
        //                             CurrentScore += effect.PositivePower : 
        //                             gameStats.GameHighScore;
        //                         break;
        //                     case EffectType.Debuff:
        //                         CurrentScore = CurrentScore - effect.NegativePower > 0 ? 
        //                             CurrentScore -= effect.NegativePower : 0;
        //                         break;
        //                 }
        //                 break;
        //             case EffectTargetType.HitPoints:
        //                 if (effect.Type == EffectType.Debuff && !IsUnitInvulnerable)
        //                 {
        //                     CurrentHp = CurrentHp - effect.NegativePower > 0 ? CurrentHp -= effect.NegativePower : 0;
        //                 }
        //                 break;
        //             default:
        //                 throw new ArgumentOutOfRangeException();
        //         }
        //     }
// }
        //
        // // TODO: Applying effects -> to separate manager
        // private IEnumerator ApplyEffectCoroutine(IEffectable effect)
        // {
        //     // Apply effect
        //     // Buff
        //     if (effect.Type == EffectType.Buff)
        //     {
        //         if (effect.BoosterType != BoosterType.None)
        //         {
        //             switch (effect.BoosterType) 
        //             {
        //                 case BoosterType.TempSpeedBoost:
        //                     SpeedMultiplier = SpeedMultiplierConst * effect.PositivePower;
        //                     break;
        //                 case BoosterType.TempInvulnerability:
        //                     IsUnitInvulnerable = true;
        //                     break;
        //             }
        //         }
        //     }
        //     
        //     // Debuff
        //     else
        //     {
        //         switch (effect.EffectTarget)
        //         {
        //             case EffectTargetType.GamePoints:
        //                 break;
        //             case EffectTargetType.HitPoints:
        //                 break;
        //             case EffectTargetType.UnitSpeed:
        //                 SpeedMultiplier = SpeedMultiplierConst / effect.NegativePower / 10;
        //                 break;
        //         }   
        //     }
        //
        //     yield return new WaitForSeconds(effect.Duration == 0 ? gameStats.BuffDuration :  effect.Duration);
        //
        //     // Cancel effect
        //     if (effect.Type == EffectType.Buff)
        //     {
        //         if (effect.BoosterType != BoosterType.None)
        //         {
        //             switch (effect.BoosterType) 
        //             {
        //                 case BoosterType.TempSpeedBoost:
        //                     SpeedMultiplier = SpeedMultiplierConst;
        //                     break;
        //                 case BoosterType.TempInvulnerability:
        //                     IsUnitInvulnerable = false;
        //                     break;
        //             }
        //         }
        //     }
        //     else
        //     {
        //         switch (effect.EffectTarget)
        //         {
        //             case EffectTargetType.GamePoints:
        //                 break;
        //             case EffectTargetType.HitPoints:
        //                 break;
        //             case EffectTargetType.UnitSpeed:
        //                 SpeedMultiplier = SpeedMultiplierConst;
        //                 break;
        //         }   
        //     }
        // }