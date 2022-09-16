using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Stats
{
    [CreateAssetMenu(fileName = "PlayerStats", menuName = "RollABall/Stats/PlayerStats", order = 0)]
    public class PlayerStats : ScriptableObject
    {
        [field: SerializeField] 
        public float MaxHp { get; set; }
        
        [field: SerializeField, Range(1f, 5f)]
        public float MaxSpeed { get; set; } = 5.0f;
    }
}