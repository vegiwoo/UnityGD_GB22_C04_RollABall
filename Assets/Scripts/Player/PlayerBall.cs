using System;
using System.Collections;
using GameDevLib.Interfaces;
using RollABall.Args;
using RollABall.Interactivity.Bonuses;
using RollABall.Interactivity.Effects;
using UnityEngine;
using static UnityEngine.Debug;

// ReSharper disable once CheckNamespace
namespace RollABall.Player
{
    public class PlayerBall : Player, GameDevLib.Interfaces.IObserver<BonusArgs>
    {
        
        #region MonoBehavior methods

        protected override void Start()
        {
            base.Start();
            transform.gameObject.tag = GameData.PlayerTag;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            bonusEvent.Attach(this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            bonusEvent.Detach(this);
        }
        
        private void FixedUpdate()
        {
            Move();
        }

        #endregion
        
        #region Functionality

        public void OnEventRaised(ISubject<BonusArgs> subject, BonusArgs args)
        {
            ApplyEffect(args.Effect);
        }

        // TODO: Applying effects -> to separate manager
        private void ApplyEffect(IEffectable effect)
        {
            Log(effect.ToString());

            // Apply effects without duration
            if (effect.Duration == 0 && effect.BoosterType == BoosterType.None)
            {
                switch (effect.EffectTarget)
                {
                    case EffectTargetType.GamePoints:
                        switch (effect.Type)
                        {
                            case EffectType.Buff:
                                CurrentScore = CurrentScore + effect.PositivePower < gameStats.GameHighScore ? 
                                    CurrentScore += effect.PositivePower : 
                                    gameStats.GameHighScore;
                                break;
                            case EffectType.Debuff:
                                CurrentScore = CurrentScore - effect.NegativePower > 0 ? 
                                    CurrentScore -= effect.NegativePower : 0;
                                break;
                        }
                        break;
                    case EffectTargetType.HitPoints:
                        if (effect.Type == EffectType.Debuff && !IsUnitInvulnerable)
                        {
                            CurrentHp = CurrentHp - effect.NegativePower > 0 ? CurrentHp -= effect.NegativePower : 0;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                StartCoroutine(ApplyEffectCoroutine(effect));
            }
        }

        // TODO: Applying effects -> to separate manager
        private IEnumerator ApplyEffectCoroutine(IEffectable effect)
        {
            // Apply effect
            // Buff
            if (effect.Type == EffectType.Buff)
            {
                if (effect.BoosterType != BoosterType.None)
                {
                    switch (effect.BoosterType) 
                    {
                        case BoosterType.TempSpeedBoost:
                            SpeedMultiplier = SpeedMultiplierConst * effect.PositivePower;
                            break;
                        case BoosterType.TempInvulnerability:
                            IsUnitInvulnerable = true;
                            break;
                    }
                }
            }
            
            // Debuff
            else
            {
                switch (effect.EffectTarget)
                {
                    case EffectTargetType.GamePoints:
                        break;
                    case EffectTargetType.HitPoints:
                        break;
                    case EffectTargetType.UnitSpeed:
                        SpeedMultiplier = SpeedMultiplierConst / effect.NegativePower / 10;
                        break;
                }   
            }

            yield return new WaitForSeconds(effect.Duration == 0 ? gameStats.BuffDuration :  effect.Duration);

            // Cancel effect
            if (effect.Type == EffectType.Buff)
            {
                if (effect.BoosterType != BoosterType.None)
                {
                    switch (effect.BoosterType) 
                    {
                        case BoosterType.TempSpeedBoost:
                            SpeedMultiplier = SpeedMultiplierConst;
                            break;
                        case BoosterType.TempInvulnerability:
                            IsUnitInvulnerable = false;
                            break;
                    }
                }
            }
            else
            {
                switch (effect.EffectTarget)
                {
                    case EffectTargetType.GamePoints:
                        break;
                    case EffectTargetType.HitPoints:
                        break;
                    case EffectTargetType.UnitSpeed:
                        SpeedMultiplier = SpeedMultiplierConst;
                        break;
                }   
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            bonusEvent.Detach(this);
        }

        #endregion
    }
}