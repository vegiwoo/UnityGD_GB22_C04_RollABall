using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Bonuses
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EffectType
    {
        Buff, Debuff
    }
}