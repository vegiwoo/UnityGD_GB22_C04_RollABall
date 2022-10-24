using System.Collections.Generic;
using RollABall.Args;
using UnityEngine;
using UnityEngine.UI;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    public partial class UIManager
    {
        #region Game actions 
        
        protected override void NewGameAction()
        {
            var rect = RadarMapImage.rectTransform.rect;
            _blipHeight  = rect.height * BlipSize / 100;
            _blipWidth   = rect.width * BlipSize / 100;
            
            var buttons = new[] { RestartButton, SaveButton, LoadButton };
            foreach (var button in buttons)
            {
                var colors = button.colors;
                colors.normalColor = colors.pressedColor = colors.selectedColor = normalColor;
                colors.highlightedColor = dangerColor;
                button.colors = colors;
            }
                
            HpLabel.color = ScoreLabel.color = SpeedLabel.color = normalColor;
            SetValues(new PlayerArgs(PlayerStats.MaxHp, false,  false, false, Vector3.one, 0));
        }
    
        protected override void SaveGameAction()
        {
            // .. 
        }
    
        protected override void LoadGameAction(SaveGameArgs args)
        {
            // ... 
        }
    
        protected override void RestartGameAction()
        {
            // ...
        }
        #endregion
    
        #region Repository actions 
        
        private static void ActivateRadarPointAction(KeyValuePair<Transform, RawImage> item)
        {
            var go = item.Value.gameObject;
            if (!go.activeInHierarchy)
            {
                item.Value.gameObject.SetActive(true);
            }
        }
            
        private static void UnactivateRadarPointAction(KeyValuePair<Transform, RawImage> item)
        {
            var go = item.Value.gameObject;
            if (go.activeInHierarchy)
            {
                item.Value.gameObject.SetActive(false);
            }
        }
            
        private static void UnactivateAndDestroyRadarPointAction(KeyValuePair<Transform, RawImage> item)
        {
            var go = item.Value.gameObject;
            go.SetActive(false); 
            Destroy(go);
        }
        
        #endregion
    }
}