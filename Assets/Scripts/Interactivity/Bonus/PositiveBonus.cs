using GameDevLib.Helpers;
using RollABall.Interactivity.Effects;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Bonuses
{
    public class PositiveBonus : InteractiveObject, IBonusRepresentable
    {
        #region Properties
        [field:SerializeField, ReadonlyField] public BonusType BonusType { get; set; }
        [field:SerializeField, ReadonlyField] public EffectType EffectType { get; set; } = EffectType.Buff;
        [field:SerializeField, ReadonlyField] public BoosterType? BoosterType { get; set; }
        public Transform PointOfPlacement { get; private set; }
        public Effect EffectOfBonus { get; set; }
        public event IInteractable<IBonusRepresentable>.InteractiveHandler InteractiveNotify;

        #endregion
        
        #region MonoBehavior methods

        protected override void Start()
        {
            base.Start();
            CompareTags.Add(GameData.PlayerTag);
        }

        #endregion

        #region Functionality
        public void Init(BonusType bonusType, Effect effect, Transform point, BoosterType? boosterType)
        {
            BonusType = bonusType;
            EffectOfBonus = effect;
            PointOfPlacement = point;
            BoosterType = boosterType;
        }

        protected override void Interaction(string tagElement)
        {
            OnGettingNotify(this, tagElement);
        }
        
        public void OnGettingNotify(IBonusRepresentable bonus, string tagElement)
        {
            InteractiveNotify?.Invoke(bonus, tagElement);
        }
        
        #endregion
    }
}