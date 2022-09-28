#nullable enable

using System;

// ReSharper disable once CheckNamespace
namespace RollABall.Args
{
    public class PlayerArgs : EventArgs
    {
        #region Properties
        
        public float CurrentHp { get; }
        public bool IsUnitInvulnerable { get; }
        public int GamePoints { get; }
        public bool IsSpeedUp { get; }
        public bool IsSpeedDown { get; }
        
        #endregion
        
        #region Constructor
        public PlayerArgs(float currentHp, bool isUnitInvulnerable,  bool isSpeedUp, bool isSpeedDown, int gamePoints = 0)
        {
            CurrentHp = currentHp;
            IsUnitInvulnerable = isUnitInvulnerable;
            IsSpeedUp = isSpeedUp;
            IsSpeedDown = isSpeedDown;
            GamePoints = gamePoints;
        }
        #endregion
    }
}