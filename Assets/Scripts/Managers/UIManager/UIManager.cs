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
    public partial class UIManager : BaseManager, GameDevLib.Interfaces.IObserver<PlayerArgs>,
        GameDevLib.Interfaces.IObserver<BonusManagerArgs>
    {
        #region Links

        [field: SerializeField] private PlayerStats PlayerStats { get; set; }
        [field: SerializeField] private PlayerEvent PlayerEvent { get; set; }

        [field: Header("Colors")] 
        [SerializeField, ColorUsage(true,true)] private Color32 normalColor = new(236, 217, 21, 255);
        [SerializeField, ColorUsage(true,true)] private Color32 dangerColor = new(236, 107, 22, 255);
        [SerializeField, ColorUsage(true,true)] private Color32 buffColor = new(166, 115, 243, 255);

        [field: Header("UI elements")]
        [field: SerializeField]
        public Text HpLabel { get; set; }

        [field: SerializeField] public Text ScoreLabel { get; set; }
        [field: SerializeField] public Text SpeedLabel { get; set; }
        [field: SerializeField] public Button RestartButton { get; set; }
        [field: SerializeField] public Button SaveButton { get; set; }
        [field: SerializeField] public Button LoadButton { get; set; }

        #endregion

        #region Properties

        private (bool isLost, string message)? IsLostGame { get; set; }
        private (bool isWin, string message)? IsWinGame { get; set; }

        #endregion

        #region Fields

        private Rect _windowRect = new(Screen.width / 2 - 90, Screen.height / 2 - 100, 180, 120);

        #endregion

        #region MonoBehaviour methods
        
        private void Update()
        {
            if (Time.frameCount % 2 == 0)
            {
                DisplayBonusesOnMap();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerEvent.Attach(this);
            BonusManagerEvent.Attach(this);

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
            else if (IsWinGame.HasValue)
            {
                message = IsWinGame.Value.message;
            }
            else
            {
                return;
            }

            GUI.Label(new Rect(_windowRect.width / 2 - 80, 30, 160, 60), message);
            if (GUI.Button(new Rect(_windowRect.width / 2 - 80, 65, 160, 30), "Exit"))
            {
                Log("Exit game");
                EditorApplication.isPlaying = false;
            }
        }

        #endregion

        #region Functionality

        private void SetValues(PlayerArgs args)
        {
            HpLabel.color = args.CurrentHp < GameStats.CriticalThreshold ? dangerColor :
                args.IsUnitInvulnerable ? buffColor : normalColor;
            HpLabel.text = $"hp: {args.CurrentHp}".ToUpper();

            ScoreLabel.color = args.GamePoints < GameStats.CriticalThreshold ? dangerColor : normalColor;
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
            BonusManagerEvent.Detach(this);

            // UI Elements
            RestartButton.onClick.RemoveListener(OnRestartButtonClick);
            SaveButton.onClick.RemoveListener(OnSaveButtonClick);
            LoadButton.onClick.RemoveListener(OnLoadButtonClick);
        }

        #endregion
    }
}