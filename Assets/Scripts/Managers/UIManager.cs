using GameDevLib.Interfaces;
using RollABall.Args;
using RollABall.Events;
using RollABall.Stats;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Debug;

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
        [field: SerializeField] public Button SaveButton { get; set; }
        [field: SerializeField] public Button LoadButton { get; set; }

        private (bool isLost, string message)? IsLostGame { get; set; }
        private (bool isWin, string message)? IsWinGame { get; set; }
        
        private Rect _windowRect = new (Screen.width / 2 - 90, Screen.height / 2 - 100, 180, 120);
        #endregion
        
        #region MonoBehaviour methods

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerEvent.Attach(this);
            
            // UI Elements
            RestartButton.onClick.AddListener(OnRestartButtonClick);
            SaveButton.onClick.AddListener(OnSaveButtonClick);
            LoadButton.onClick.AddListener(OnLoadButtonClick);
        }
        
        private void OnGUI()
        {
            if (IsLostGame.HasValue)
            {
                _windowRect = GUI.Window(0, _windowRect, WindowMakeFunction, "You lose");
            }
            else if (IsWinGame.HasValue)
            {
                _windowRect = GUI.Window(0, _windowRect, WindowMakeFunction, "You win");
            }
        }

        private void WindowMakeFunction(int windowId)
        {
            string message;
            if (IsLostGame.HasValue)
            {
                message = IsLostGame.Value.message;
            } 
            else if(IsWinGame.HasValue)
            {
                message = IsWinGame.Value.message;
            }
            else
            {
                return;
            }

            GUI.Label(new Rect(_windowRect.width/2 - 80, 30, 160, 60), message);
            if (GUI.Button(new Rect(_windowRect.width / 2 - 80, 65, 160, 30), "Exit"))
            {
                Log("Exit game");
                EditorApplication.isPlaying = false;
            }
        }
        
        #endregion 
        
        #region Functionality
        
        protected override void InitManager()
        {
            var buttons = new[] { RestartButton, SaveButton, LoadButton };
            foreach (var button in buttons)
            {
                var colors = button.colors;
                colors.normalColor = colors.pressedColor = colors.selectedColor = normalColor;
                colors.highlightedColor = dangerColor;
                button.colors = colors;
            }
            
            HpLabel.color = ScoreLabel.color = SpeedLabel.color = normalColor;
            SetValues(new PlayerArgs(PlayerStats.MaxHp, false,  false, false, 0));
        }

        private void OnRestartButtonClick() => GameEvent.Notify(new CurrentGameArgs(true, false, false));
        
        private void OnSaveButtonClick() => GameEvent.Notify(new CurrentGameArgs(false, true, false));
        
        private void OnLoadButtonClick() => GameEvent.Notify(new CurrentGameArgs(false, false, true));
        
        // Event handler for PlayerEvent
        public void OnEventRaised(ISubject<PlayerArgs> subject, PlayerArgs args)
        {
            SetValues(args);
        }
        
        // Event handler for CurrentGameEvent
        public override void OnEventRaised(ISubject<CurrentGameArgs> subject, CurrentGameArgs args)
        {
            // Lost game
            if (args.IsLostGame is { isLost: true })
            {
                Log(args.IsLostGame.Value.message);
                IsLostGame = args.IsLostGame.Value;
            }
            else if (args.IsWinGame is { isWin: true })
            {
                Log(args.IsWinGame.Value.message);
                IsWinGame = args.IsWinGame.Value;
            }
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
            
            // UI Elements
            RestartButton.onClick.RemoveListener(OnRestartButtonClick);
            SaveButton.onClick.RemoveListener(OnSaveButtonClick);
            LoadButton.onClick.RemoveListener(OnLoadButtonClick);
        }
        
        #endregion
    }
}