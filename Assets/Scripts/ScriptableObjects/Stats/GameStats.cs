using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Stats
{
    [CreateAssetMenu(fileName = "GameStats", menuName = "RollABall/Stats/GameStats", order = 2)]
    public class GameStats : ScriptableObject
    {
        [field: SerializeField] public int GameHighScore { get; set; } = 100;
        
        [field: SerializeField, Range(10, 50), Tooltip("Threshold at which values (game points, hp) are considered low")]
        public float CriticalThreshold { get; set; } = 25;

        [field: SerializeField, Range(10f, 30.0f)] public float BuffDuration { get; set; } = 15.0f;
    }
}
