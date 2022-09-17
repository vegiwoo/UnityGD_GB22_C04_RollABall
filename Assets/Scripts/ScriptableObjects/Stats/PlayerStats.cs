using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Stats
{
    [CreateAssetMenu(fileName = "PlayerStats", menuName = "RollABall/Stats/PlayerStats", order = 0)]
    public class PlayerStats : ScriptableObject
    {
        [field: SerializeField] public float MaxHp { get; set; }
        [field: SerializeField, Range(5f, 10f)] public float MaxSpeed { get; set; } = 7;
    }
}