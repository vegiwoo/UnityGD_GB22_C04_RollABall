using System;
using GameDevLib.Interfaces;
using RollABall.Args;
using RollABall.Interactivity.Bonuses;
using RollABall.Interactivity.Effects;

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
        
        private void ApplyEffect(Effect effect)
        {
            // Apply effects without duration
            if (effect.Duration == 0)
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
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        break;
                    case EffectTargetType.UnitHp:
                        break;
                    case EffectTargetType.UnitSpeed:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                // Apply effects with duration (coroutines)
            }
            
            var args = new PlayerArgs(CurrentHp, (int)CurrentScore);
            playerEvent.Notify(args);
        }
        #endregion
    }
}