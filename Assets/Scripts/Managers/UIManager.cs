using RollABall.Args;
using UnityEngine;
using UnityEngine.UI;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    public class UIManager : MonoBehaviour
    {
        #region Links

        [SerializeField] 
        private Text hpLabel;
        
        [SerializeField] 
        private Text scoreLabel;
        
        #endregion
        
        #region Functionality

        public void SetValues(PlayerArgs args)
        {
            hpLabel.text = $"hp: {args.CurrentHp}".ToUpper();
        }
        
        #endregion
    }
}