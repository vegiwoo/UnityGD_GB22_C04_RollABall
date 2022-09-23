using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Bonuses
{
    public class NegativeBonus : InteractiveObject, IBonusable
    {
        #region Properties

        public Guid Id { get; }
        public Transform Point { get; set; }
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

        protected override void Interaction(string tagElement)
        { 
            InteractiveNotify?.Invoke(this, tagElement);
        }
        
        public void Dispose()
        {
            CompareTags = null;
            InteractiveNotify = null;
        }

        #endregion
    }
}