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

        protected override void Update()
        {
            Velocity = playerRb.velocity.magnitude;
            base.Update();
        }

        private void FixedUpdate()
        {
            Move();
        }
        
        #endregion
        
        #region Functionality

        public override void SetGamePoints(float points, bool increase)
        {
            GamePoints = increase switch
            {
                true => GamePoints + points >= gameStats.GameHighScore
                    ? gameStats.GameHighScore
                    : GamePoints += points,
                false => GamePoints - points > 0
                    ? GamePoints -= points
                    : 0
            };
        }

        public override void SetHitPoints(float? hp, bool? increase, bool? isInvulnerable = null)
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

        public override void SetSpeed(float? multiplier, bool? increase, bool? cancelEffect = null)
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