using System;
using GameDevLib.Interfaces;
using RollABall.Args;
using RollABall.Events;
using RollABall.Stats;
using UnityEngine;
using UnityEngine.UI;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    public class UIManager : MonoBehaviour, GameDevLib.Interfaces.IObserver<PlayerArgs>, IDisposable
    {
        #region Links
        [field:Header("Stats")]
        [field: SerializeField] private GameStats GameStats { get; set; }
        [field: SerializeField] private PlayerStats PlayerStats { get; set; }
        
        [field:Header("Events")]
        [field:SerializeField] private PlayerEvent playerEvent;

        [field:Header("Colors")]
        [SerializeField] private Color32 normalColor = new (236, 217, 21, 255);
        [SerializeField] private Color32 dangerColor = new (236, 107, 22, 255);
        [SerializeField] private Color32 buffColor = new (166, 115, 243, 255);

        #endregion
        
        #region Properties
        [field:Header("UI elements")]
        [field: SerializeField] public Text HpLabel { get; set; }
        [field: SerializeField] public Text ScoreLabel { get; set; }
        [field: SerializeField] public Text SpeedLabel { get; set; }

        #endregion
        

        #region MonoBehaviour methods

        private void OnEnable()
        {
            playerEvent.Attach(this);

            HpLabel.color = ScoreLabel.color = SpeedLabel.color = normalColor;
            SetValues(new PlayerArgs(PlayerStats.MaxHp, false, 0,false, false, 0 ));
        }

        private void OnDisable()
        {
            Dispose();
        }

        #endregion 
        #region Functionality
        
        public void Dispose()
        {
            playerEvent.Detach(this);
        }
        
        public void OnEventRaised(ISubject<PlayerArgs> subject, PlayerArgs args)
        {
            SetValues(args);
        }

        private void SetValues(PlayerArgs args)
        {
            HpLabel.color = args.CurrentHp < GameStats.CriticalThreshold ? dangerColor : args.IsUnitInvulnerable ? buffColor : normalColor;
            HpLabel.text = $"hp: {args.CurrentHp}".ToUpper();

            ScoreLabel.color = args.GamePoints < GameStats.CriticalThreshold  ? dangerColor : normalColor;
            ScoreLabel.text = $"score: {args.GamePoints}/{GameStats.GameHighScore}".ToUpper();
            
            SpeedLabel.color = args.IsSpeedUp == false && args.IsSpeedDown == false ? normalColor :
                args.IsSpeedUp ? buffColor : dangerColor;
            var speedModeLabel = args.IsSpeedUp == false && args.IsSpeedDown == false ? "normal" :
                args.IsSpeedUp ? "high" : "low";
            SpeedLabel.text = $"speed: {speedModeLabel}".ToUpper();
        }
        #endregion
    }
}