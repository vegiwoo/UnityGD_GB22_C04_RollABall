using System;
using GameDevLib.Helpers;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Bonuses
{
    public class NegativeBonus : InteractiveObject, IBonusable
    {
        #region Properties

        public Guid Id { get; } = new ();
        [field: SerializeField, ReadonlyField] public BonusType BonusType { get; set; }
        [field: SerializeField, ReadonlyField] public EffectType EffectType { get; set; } = EffectType.Debuff;
        [field: SerializeField, ReadonlyField] public BoosterType? BoosterType { get; set; }
        public Transform Point { get; private set; }
        public IEffectable Effect { get; set; }

        public event IInteractable<IBonusable>.InteractiveHandler InteractiveNotify;
        
        #endregion
        
        #region MonoBehavior methods

        protected override void Start()
        {
            base.Start();
            CompareTags.Add(GameData.PlayerTag);
        }

        #endregion

        #region Functionality

        public void Init(BonusType bonusType, IEffectable effect, Transform point)
        {
            BonusType = bonusType;
            BoosterType = effect.BoosterType;
            Effect = effect;
            Point = point;
        }
        
        protected override void Interaction(string tagElement)
        { 
            OnGettingNotify(this, tagElement);
        }

        public void OnGettingNotify(IBonusable bonus, string tagElement)
        {
            InteractiveNotify?.Invoke(bonus, tagElement);
        }

        public void Dispose()
        {
            CompareTags = null;
            InteractiveNotify = null;
        }

        #endregion
    }
}

//  public Guid Id { get; } = new ();
//         [field: SerializeField, ReadonlyField] public BonusType BonusType { get; set; } зачем?
//    [field: SerializeField, ReadonlyField] public EffectType EffectType { get; set; } = EffectType.Debuff; - брать из эффекта 
//  [field: SerializeField, ReadonlyField] public BoosterType? BoosterType { get; set; } зачем 
//         public Transform Point { get; private set; }
//   public IEffectable Effect { get; set; }