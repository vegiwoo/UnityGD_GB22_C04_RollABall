using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Effects
{
    /// <summary>
    /// Targets for applying buff effect
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EffectTargetType
    {
        GamePoints, HitPoints, UnitSpeed, Rebirth
    }
}