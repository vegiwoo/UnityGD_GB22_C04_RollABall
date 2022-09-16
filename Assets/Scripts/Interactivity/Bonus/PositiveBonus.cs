using GameDevLib.Helpers;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Bonuses
{
    public class PositiveBonus : InteractiveObject, IBonusRepresentable
    {
        #region Properties
        
        public BonusType Type { get; } = BonusType.Positive;
        public NegativeBonusType? NegativeType { get; } = null;
        
        [field:SerializeField,ReadonlyField]
        public PositiveBonusType? PositiveType { get; set; }
        
        [field:SerializeField,ReadonlyField]
        public BoosterType? BoosterType { get; set; }

        public BonusActionPointType ActionPointType { get; set; }

        public Transform PointOfPlacement { get; private set; }
        
        [field:SerializeField, ReadonlyField]
        public float Power { get; set; }

        public float Duration { get; private set; }

        #endregion

        #region Functionality
        public void PositiveInit(PositiveBonusType positiveType,  Transform pointOfPlacement)
        {
            PositiveType = positiveType.RandomValue(positiveType);
            PointOfPlacement = pointOfPlacement;

            switch (positiveType)
            {
                case PositiveBonusType.GamePoints:
                    ActionPointType = BonusActionPointType.GamePoints;
                    Power = 10;
                    Duration = 0;
                    break;
                case PositiveBonusType.Booster:
                    var buster = Bonuses.BoosterType.Immortality;
                    var randomBuster = buster.RandomValue(Bonuses.BoosterType.Immortality);
                    BoosterType = randomBuster;

                    switch (BoosterType)
                    {
                        case Bonuses.BoosterType.TempSpeedBoost:
                            ActionPointType = BonusActionPointType.Speed;
                            Power = 2;
                            Duration = 10f;
                            break;
                        case Bonuses.BoosterType.Immortality:
                            ActionPointType = BonusActionPointType.Hp;
                            Power = 1000f;
                            Duration = 10f;
                            break;
                        case null:
                            break;
                    }

                    break;
            }
        }

        public void NegativeInit(NegativeBonusType negativeType, Transform point)
        {
            // Not implement
        }
        protected override void Interaction()
        {
       
        }
        
        #endregion
    }
}