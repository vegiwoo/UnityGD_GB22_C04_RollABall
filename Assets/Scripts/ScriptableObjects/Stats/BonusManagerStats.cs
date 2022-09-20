using GameDevLib.Helpers;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Stats
{
    [CreateAssetMenu(fileName = "BonusStats", menuName = "RollABall/Stats/BonusStats", order = 1)]
    public class BonusManagerStats : ScriptableObject
    {
        [field: Header("Common")] 
        [field: SerializeField, Range(0.1f,5.0f)] public float DelayAppearance { get; set; } = 0.50f;
        [field: SerializeField, Range(1f,10.0f)] public float DelayAfterRemove { get; set; } = 5.0f;

        [field:Header("Speed Effect")]
        [field: SerializeField, Range(1f, 20f)] public float SpeedEffectDuration { get; set; } = 10.0f;
        [field: SerializeField, Range(1.5f, 3f)] public float SpeedEffectMultiplier { get; set; } = 1.5f;

        [field:Header("Game Points Effect")]
        [field: SerializeField, ReadonlyField] public float GpEffectDuration { get; set; } = 0.0f;
        [field: SerializeField, Range(5f, 20f)] public float GpEffectValue { get; set; } = 10.0f;

        [field: Header("Unit Hp Effect")]
        [field: SerializeField, ReadonlyField] public float HpEffectDuration { get; set; } = 0.0f;
        [field: SerializeField, Range(5f, 20f)] public float HpEffectValue { get; set; } = 10.0f;
    }
}