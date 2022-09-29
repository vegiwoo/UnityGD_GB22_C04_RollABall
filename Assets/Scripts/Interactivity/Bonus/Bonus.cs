using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Bonuses
{
    public class Bonus : InteractiveObject, IBonusable
    {
        #region Properties
        
        public event IInteractable<IBonusable>.InteractiveHandler InteractiveNotify;
        public Transform Point { get; set; }
        public IEffectable Effect { get; set; }
        
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