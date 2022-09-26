using System;
using RollABall.Args;

// ReSharper disable once CheckNamespace

namespace RollABall.Player
{
    public class PlayerBall : Player
    {
        #region MonoBehavior methods

        protected override void Start()
        {
            base.Start();
            transform.gameObject.tag = GameData.PlayerTag;
        }

        private void Update()
        {
            //Velocity = playerRb.velocity.magnitude;
        }

        private void FixedUpdate()
        {
            Move();
        }
        
        #endregion
        
        #region Functionality

        protected override void SetGamePoints(float? points, bool? increase)
        {
            if (points.HasValue && increase.HasValue)
            {
                GamePoints = increase.Value switch
                {
                    true => GamePoints + points >= gameStats.GameHighScore
                        ? gameStats.GameHighScore
                        : GamePoints += points.Value,
                    false => GamePoints - points > 0
                        ? GamePoints -= points.Value
                        : 0
                };
            }
        }

        protected override void SetHitPoints(float? hp, bool? increase, bool? isInvulnerable = null)
        {
            // Set Invulnerable
            if (isInvulnerable.HasValue)
            {
                IsUnitInvulnerable = isInvulnerable.Value;
            }

            // Set HP (only !increase)
            if (hp.HasValue && increase.HasValue && !increase.Value && !IsUnitInvulnerable)
            {
                CurrentHp = CurrentHp - hp.Value > 0
                    ? CurrentHp -= hp.Value
                    : 0;
            }
        }

        protected override void SetSpeed(float? multiplier, bool? increase, bool? cancelEffect = null)
        {
            // Cancel effect
            if (cancelEffect.HasValue && cancelEffect.Value)
            {
                SpeedMultiplier = SpeedMultiplierConst;
                return;
            }

            if (!multiplier.HasValue || !increase.HasValue) return;
            
            // Apply effect
            switch (increase.Value)
            {
                case true:
                    if (SpeedMultiplier < SpeedMultiplierConst || Math.Abs(SpeedMultiplier - SpeedMultiplierConst) < 0.1f)
                    {
                        SpeedMultiplier = SpeedMultiplierConst * multiplier.Value;
                    }
                    break;
                case false:
                    if (SpeedMultiplier > SpeedMultiplierConst || Math.Abs(SpeedMultiplier - SpeedMultiplierConst) < 0.1f)
                    {
                        SpeedMultiplier = SpeedMultiplierConst / multiplier.Value;
                    }
                    break;
            }
        }
        
        protected override void SendNotify()
        {
            var isSpeedUp = SpeedMultiplier > SpeedMultiplierConst;
            var isSpeedDown = SpeedMultiplier < SpeedMultiplierConst;

            var args = new PlayerArgs(
                CurrentHp, 
                IsUnitInvulnerable, 
                Velocity, 
                isSpeedUp, 
                isSpeedDown,
                (int)GamePoints
            );
            
            playerEvent.Notify(args);
        }

        #endregion
    }
}