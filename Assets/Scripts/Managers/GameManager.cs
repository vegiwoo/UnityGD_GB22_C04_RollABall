using GameDevLib.Interfaces;
using RollABall.Args;
using RollABall.Events;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    public class GameManager : MonoBehaviour, GameDevLib.Interfaces.IObserver<PlayerArgs>
    {
        #region Links
        
        [field: SerializeField]
        private BonusManager BonusManager { get; set; }
        
        [field: SerializeField]
        private UIManager UIManager { get; set; }
        
        [SerializeField] 
        private PlayerEvent playerEvent;
        
        #endregion
        
        #region MonoBehaviour methods

        private void OnEnable()
        {
            playerEvent.Attach(this);
        }

        private void OnDisable()
        {
            playerEvent.Detach(this);
        }

        #endregion
        
        #region Functionality
        
        public void OnEventRaised(ISubject<PlayerArgs> subject, PlayerArgs args)
        {
            UIManager.SetValues(args);
        }
        
        #endregion
    }
}