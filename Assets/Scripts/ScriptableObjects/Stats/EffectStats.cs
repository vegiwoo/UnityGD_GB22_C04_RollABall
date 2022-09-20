using RollABall.Managers;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Stats
{
    [CreateAssetMenu(fileName = "EffectStats", menuName = "RollABall/Stats/EffectStats", order = 1)]
    public class EffectStats : ScriptableObject
    {
        [field: Header("Common")] 
        [field: SerializeField, Range(0.1f,5.0f)] public float DelayAppearance { get; set; } = 0.50f;
        [field: SerializeField, Range(1f,10.0f)] public float DelayAfterRemove { get; set; } = 5.0f;

        [SerializeField] public Effect[] effects;
    }
}