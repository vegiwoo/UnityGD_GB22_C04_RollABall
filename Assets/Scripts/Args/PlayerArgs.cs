#nullable enable

using System;
using Newtonsoft.Json;
using RollABall.Models;

// ReSharper disable once CheckNamespace
namespace RollABall.Args
{
    [Serializable]
    public class PlayerArgs : EventArgs
    {
        #region Properties
        
        public float CurrentHp { get; }
        public bool IsUnitInvulnerable { get; }
        public int GamePoints { get; }
        public Point Point { get; }
        
        [JsonIgnore]
        public bool IsSpeedUp { get; }
        [JsonIgnore]
        public bool IsSpeedDown { get; }
        
        #endregion
        
        #region Constructor
       
        public PlayerArgs(float currentHp, bool isUnitInvulnerable,  bool isSpeedUp, bool isSpeedDown, UnityEngine.Vector3 position, int gamePoints = 0)
        {
            CurrentHp = currentHp;
            IsUnitInvulnerable = isUnitInvulnerable;
            IsSpeedUp = isSpeedUp;
            IsSpeedDown = isSpeedDown;
            GamePoints = gamePoints;
            Point = new Point(position: position);
        }
        
        [JsonConstructor]
        public PlayerArgs(float currentHp, bool isUnitInvulnerable, int gamePoints,  Point point)
        {
            CurrentHp = currentHp;
            IsUnitInvulnerable = isUnitInvulnerable;
            GamePoints = gamePoints;
            Point = point;
        }
        
        public override string ToString()
        {
            return $"CurrentHp:{CurrentHp}, IsUnitInvulnerable:{IsUnitInvulnerable}, GamePoints:{GamePoints}, Point:{Point}";
        }
        
        #endregion
    }
}