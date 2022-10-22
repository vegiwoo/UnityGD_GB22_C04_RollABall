using System;
using RollABall.Args;
using RollABall.Infrastructure.Memento;

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

        protected override void OnEnable()
        {
            base.OnEnable();
            
            // // Memento pattern - init caretaker for organizer.
            // Caretaker.Init(this, "Player", "PlayerMemento");
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
            State = MakeState();
            playerEvent.Notify(State);
        }
        
        // Memento pattern methods

        public override PlayerArgs MakeState()
        {
            var isSpeedUp = SpeedMultiplier > SpeedMultiplierConst;
            var isSpeedDown = SpeedMultiplier < SpeedMultiplierConst;

            return new PlayerArgs(
                CurrentHp,
                IsUnitInvulnerable,
                isSpeedUp,
                isSpeedDown,
                transform.position,
                (int)GamePoints
            );
        }
        
        // public IMemento<PlayerArgs> Save()
        // {
        //     State = MakeState();
        //     return new Memento<PlayerArgs>(State, "Player");
        // }
        //
        // public void Load(IMemento<PlayerArgs> memento)
        // {
        //     if (memento is not Memento<PlayerArgs>)
        //     {
        //         throw new Exception("Unknown memento class " + memento.ToString());
        //     }
        //     
        //     State = memento.State;
        //     InitPlayer(true);
        // }
        
        #endregion
    }
}