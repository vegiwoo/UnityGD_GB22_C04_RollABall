using GameDevLib.Interfaces;
using RollABall.Args;
using RollABall.Events;
using RollABall.Stats;
using UnityEngine;
using UnityEngine.UI;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    public class UIManager : BaseManager, IObserver<PlayerArgs>
    {
        #region Links
        
        [field: SerializeField] private PlayerStats PlayerStats { get; set; }
        [field:SerializeField] private PlayerEvent PlayerEvent { get; set; }

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
        [field: SerializeField] public Button RestartButton { get; set; }

        #endregion
        
        #region MonoBehaviour methods

        protected override void OnEnable()
        {
            PlayerEvent.Attach(this);
            RestartButton.onClick.AddListener(OnRestartButtonClick);
            
            InitManager();
        }
        
        #endregion 
        
        #region Functionality
        
        private void InitManager()
        {
            var colors = RestartButton.colors;
            colors.normalColor = colors.pressedColor = colors.selectedColor = normalColor;
            colors.highlightedColor = dangerColor;
            RestartButton.colors = colors;

            HpLabel.color = ScoreLabel.color = SpeedLabel.color = normalColor;
            SetValues(new PlayerArgs(PlayerStats.MaxHp, false,  false, false, 0));
        }

        private void OnRestartButtonClick()
        {
            GameEvent.Notify(new CurrentGameArgs(true));
        }
        
        // Event handler for PlayerEvent
        public void OnEventRaised(ISubject<PlayerArgs> subject, PlayerArgs args)
        {
            SetValues(args);
        }
        
        // Event handler for CurrentGameEvent
        public override void OnEventRaised(ISubject<CurrentGameArgs> subject, CurrentGameArgs args)
        {
           // Do something ...
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
        
        public override void Dispose()
        {
            base.Dispose();
            PlayerEvent.Detach(this);
            RestartButton.onClick.RemoveAllListeners();
        }
        
        #endregion
    }
}