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
        private void ApplyEffect(Effect effect)
        {
            Log(effect);

            // Apply effects without duration
            if (effect.Duration == 0 && !effect.BoosterType.HasValue)
            {
                switch (effect.EffectTarget)
                {
                    case EffectTargetType.GamePoints:
                        switch (effect.Type)
                        {
                            case EffectType.Buff:
                                CurrentScore = CurrentScore + effect.Power < gameStats.GameHighScore ? 
                                    CurrentScore += effect.Power : 
                                    gameStats.GameHighScore;
                                break;
                            case EffectType.Debuff:
                                CurrentScore = CurrentScore - effect.Power > 0 ? 
                                    CurrentScore -= effect.Power : 0;
                                break;
                        }
                        break;
                    case EffectTargetType.UnitHp:
                        if (effect.Type == EffectType.Debuff && !IsUnitInvulnerable)
                        {
                            CurrentHp = CurrentHp - effect.Power > 0 ? CurrentHp -= effect.Power : 0;
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
        private IEnumerator ApplyEffectCoroutine(Effect effect)
        {
            // Apply effect
            // Buff
            if (effect.Type == EffectType.Buff)
            {
                if (effect.BoosterType.HasValue)
                {
                    switch (effect.BoosterType.Value) 
                    {
                        case BoosterType.TempSpeedBoost:
                            SpeedMultiplier = SpeedMultiplierConst * effect.Power;
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
                    case EffectTargetType.UnitHp:
                        break;
                    case EffectTargetType.UnitSpeed:
                        SpeedMultiplier = SpeedMultiplierConst / effect.Power / 10;
                        break;
                }   
            }

            yield return new WaitForSeconds(effect.Duration == 0 ? gameStats.BuffDuration :  effect.Duration);

            // Cancel effect
            if (effect.Type == EffectType.Buff)
            {
                if (effect.BoosterType.HasValue)
                {
                    switch (effect.BoosterType.Value) 
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
                    case EffectTargetType.UnitHp:
                        break;
                    case EffectTargetType.UnitSpeed:
                        SpeedMultiplier = SpeedMultiplierConst;
                        break;
                }   
            }
        }
        
        #endregion
    }
}