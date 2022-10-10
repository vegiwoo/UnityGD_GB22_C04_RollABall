using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Bonuses
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum BoosterType
    {
        None,
        /// <summary>
        /// Temporary increase in movement speed.
        /// </summary>
        TempSpeedBoost, 
        
        /// <summary>
        /// Temporary invulnerability.
        /// </summary>
        TempInvulnerability
    }
}