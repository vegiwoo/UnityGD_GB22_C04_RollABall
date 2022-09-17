using RollABall.Args;
using RollABall.Stats;
using UnityEngine;
using UnityEngine.UI;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    public class UIManager : MonoBehaviour
    {
        #region Links
        [field:Header("Stats")]
        [field: SerializeField] private GameStats GameStats { get; set; }
        
        [field:Header("Links")]
        [SerializeField] private Text hpLabel;
        [SerializeField] private Text scoreLabel;
        [SerializeField] private Text speedLabel;
        
        [field:Header("Colors")]
        [SerializeField] private Color32 normalColor = new (236, 217, 21, 255);
        [SerializeField] private Color32 dangerColor = new (236, 107, 22, 255);
        [SerializeField] private Color32 buffColor = new (166, 115, 243, 255);

        #endregion
        
        #region Constanst and variables
        // ...
        #endregion
        
        #region MonoBehaviour methods

        private void Start()
        {
            hpLabel.color = scoreLabel.color =  speedLabel.color = normalColor;
            
            scoreLabel.text = $"score: {0}/{GameStats.GameHighScore}".ToUpper();
            speedLabel.text = $"speed: 0".ToUpper();
        }

        #endregion

        #region Functionality

        public void SetValues(PlayerArgs args)
        {
            hpLabel.color = args.CurrentHp < GameStats.CriticalThreshold ? dangerColor : args.IsUnitInvulnerable ? buffColor : normalColor;
            hpLabel.text = $"hp: {args.CurrentHp}".ToUpper();
            
            scoreLabel.color = args.GamePoints < GameStats.CriticalThreshold  ? dangerColor : normalColor;
            scoreLabel.text = $"score: {args.GamePoints}/{GameStats.GameHighScore}".ToUpper();

            speedLabel.color = args.IsSpeedUp == false && args.IsSpeedDown == false ? normalColor :
                args.IsSpeedUp ? buffColor : dangerColor;
            speedLabel.text = $"speed: {args.Velocity}".ToUpper();
        }
        
        #endregion
    }
}