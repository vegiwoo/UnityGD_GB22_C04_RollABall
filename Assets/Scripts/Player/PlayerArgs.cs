#nullable enable

using System;

// ReSharper disable once CheckNamespace
namespace RollABall.Args
{
    public struct PlayerArgs
    {
        #region Properties
        public float CurrentHp { get; }
        public bool IsUnitInvulnerable { get; }
        public int GamePoints { get; }
        public double Velocity { get; }
        
        public bool IsSpeedUp { get; }
        public bool IsSpeedDown { get; }
        #endregion
        
        #region Constructor
        public PlayerArgs(float currentHp, bool isUnitInvulnerable,  float velocity, bool isSpeedUp, bool isSpeedDown, int gamePoints = 0)
        {
            CurrentHp = currentHp;
            IsUnitInvulnerable = isUnitInvulnerable;
            Velocity = Math.Round(velocity, 2);
            IsSpeedUp = isSpeedUp;
            IsSpeedDown = isSpeedDown;
            GamePoints = gamePoints;
        }
        #endregion
    }
}