using System;
using GameDevLib.Helpers;
using JetBrains.Annotations;
using RollABall.Interactivity.Effects;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Bonuses
{
    public class NegativeBonus : InteractiveObject, IBonusRepresentable
    {
        #region Properties

        public Guid Id { get; } = new ();
        [field: SerializeField, ReadonlyField] public BonusType BonusType { get; set; }
        [field: SerializeField, ReadonlyField] public EffectType EffectType { get; set; } = EffectType.Debuff;
        [field: SerializeField, ReadonlyField] public BoosterType? BoosterType { get; set; } = null;
        public Transform Point { get; private set; }
        public Effect Effect { get; set; }

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
        
        public bool Equals( [CanBeNull] IBonusRepresentable other)  
        {        
            return other != null && Id == other.Id && Point == other.Point;
        }  
        
        public void Init(BonusType bonusType,  Effect effect, Transform point, BoosterType? boosterType = null)
        {
            BonusType = bonusType;
            BoosterType = boosterType;
            Effect = effect;
            Point = point;
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