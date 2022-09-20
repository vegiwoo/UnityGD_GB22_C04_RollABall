using System;
using GameDevLib.Helpers;
using JetBrains.Annotations;
using RollABall.Interactivity.Effects;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Bonuses
{
    public class PositiveBonus : InteractiveObject, IBonusable
    {
        #region Properties
        public Guid Id { get; } = new ();
        [field:SerializeField, ReadonlyField] public BonusType BonusType { get; set; }
        [field:SerializeField, ReadonlyField] public EffectType EffectType { get; set; } = EffectType.Buff;
        [field:SerializeField, ReadonlyField] public BoosterType? BoosterType { get; set; }
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
            Effect = effect;
            Point = point;
            BoosterType = effect.BoosterType;
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