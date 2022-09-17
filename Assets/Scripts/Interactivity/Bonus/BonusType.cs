
// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Bonuses
{
    public enum BonusType
    {
        // Positive
        
        /// <summary>
        /// Getting game points
        /// </summary>
        Gift, 
        
        /// <summary>
        /// Getting booster
        /// </summary>
        Booster,
        
        // Negative
        
        /// <summary>
        /// Temporary slowdown.
        /// </summary>
        TempSlowdown, 
        
        /// <summary>
        /// Theft (loss of game points)
        /// </summary>
        Theft, 
        
        /// <summary>
        /// Wound of unit
        /// </summary>
        Wound
    }

}