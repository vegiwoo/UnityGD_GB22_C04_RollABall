using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Stats
{
    [CreateAssetMenu(fileName = "GameStats", menuName = "RollABall/Stats/GameStats", order = 2)]
    public class GameStats : ScriptableObject
    {
        [field: SerializeField] public Vector3 PlayerSpawnPoint { get; set; }
        [field: SerializeField] public int GameHighScore { get; set; } = 100;
        
        [field: SerializeField, Range(10, 50), Tooltip("Threshold at which values (game points, hp) are considered low")]
        public float CriticalThreshold { get; set; } = 15;
    }
}