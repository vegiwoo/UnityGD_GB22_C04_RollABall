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
        [field: SerializeField, Range(2f, 4f)] public float SpeedEffectMultiplier { get; set; } = 2.0f;

        [field:Header("Game Points Effect")]
        [field: SerializeField, ReadonlyField] public float GamePointsEffectDuration { get; set; } = 0.0f;
        [field: SerializeField, Range(5f, 20f)] public float GamePointsEffectValue { get; set; } = 10.0f;

        [field: Header("Unit Hp Effect")]
        [field: SerializeField, ReadonlyField] public float UnitHpEffectDuration { get; set; } = 0.0f;
        [field: SerializeField, Range(5f, 20f)] public float UnitHpEffectMultiplier { get; set; } = 10.0f;
    }
}