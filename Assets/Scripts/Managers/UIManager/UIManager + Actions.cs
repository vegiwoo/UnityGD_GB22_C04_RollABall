using System.Collections.Generic;
using RollABall.Args;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Debug;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    public partial class UIManager
    {
        #region Game actions 
        
        public override void NewGameAction()
        {
            _radarWidth  = RadarMapImage.GetComponent<RectTransform>().rect.width;
            _radarHeight = RadarMapImage.GetComponent<RectTransform>().rect.height;
            _blipHeight  = _radarHeight * BlipSize/100;
            _blipWidth   = _radarWidth * BlipSize/100;
            
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
    
        public override void SaveGameAction()
        {
            // .. 
        }
    
        public override void LoadGameAction(SaveGameArgs args)
        {
            // ... 
        }
    
        public override void RestartGameAction()
        {
            // ...
        }

        public override void LostGameAction()
        {
            Log(GameData.LostGameMessage);
        }

        public override void WonGameAction()
        {
            Log(GameData.WonGameMessage);
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