// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Bonuses
{
    /// <summary>
    /// Description of  characteristics of a specific bonus.
    /// </summary>
    public struct BonusFeature
    {
        public BonusType Type { get; }
        public NegativeBonusType? NegativeType { get; }
        public PositiveBonusType? PositiveType { get; }
        public BoosterType? BoosterType { get; }
        public float Power { get; }

        public BonusFeature(BonusType bonusType, NegativeBonusType negativeBonusType, float power)
        {
            Type = bonusType;
            NegativeType = negativeBonusType;
            Power = power;
            PositiveType = null;
            BoosterType = null;
        }
    }


}