using System;
using System.Collections.Generic;
using System.Linq;
using GameDevLib.Interfaces;
using RollABall.Args;
using RollABall.Events;
using RollABall.ScriptableObjects;
using RollABall.Stats;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Debug;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    public class UIManager : BaseManager, GameDevLib.Interfaces.IObserver<PlayerArgs>, GameDevLib.Interfaces.IObserver<BonusManagerArgs>
    {
        #region Links
        
        [field: SerializeField] private PlayerStats PlayerStats { get; set; }
        [field:SerializeField] private PlayerEvent PlayerEvent { get; set; }

        [field:Header("Colors")]
        [SerializeField] private Color32 normalColor = new (236, 217, 21, 255);
        [SerializeField] private Color32 dangerColor = new (236, 107, 22, 255);
        [SerializeField] private Color32 buffColor = new (166, 115, 243, 255);
        
        [field:Header("Radar map")]
        [field: SerializeField] private RawImage RadarMapImage { get; set; }
        [field: SerializeField] private RawImage BonusPointImage { get; set; }
        [field: SerializeField] private BonusManagerEvent BonusManagerEvent { get; set; }
        [field: SerializeField] private  RadarObjectsRepository RadarObjectsRepository { get; set; }

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
        
        // Radar map
        private Transform PlayerPosition { get; set; }

        #endregion
        
        #region Fields
        
        public float radarDistance = 15, blipSize = 10;
        public bool usePlayerDirection = true;
        public Transform player;

        private float _radarWidth, _radarHeight, _blipWidth, _blipHeight;
        
        #endregion

        #region MonoBehaviour methods

        protected override void Start()
        {
            base.Start();
            
            var rect = RadarMapImage.rectTransform.rect;
            _blipHeight  = rect.height * blipSize / 100;
            _blipWidth   = rect.width * blipSize / 100;
        }

        private void Update()
        {
            if (Time.frameCount % 2 == 0)
            {
                DisplayBonusesOnMap();
            }
        }

        private void DisplayBonusesOnMap()
        {
            var playerPos = player.position;

            foreach (var target in RadarObjectsRepository
                         .FindAll()
                         .Where(i => i.Value.gameObject.activeInHierarchy))
            {
                var targetPos = target.Key.position;
                var distanceToTarget = Vector3.Distance(targetPos, playerPos);

                if (!(distanceToTarget <= radarDistance)) continue;
                
                var normalisedTargetPosition = NormalisedPosition(playerPos, targetPos);
                var bonusPointPosition = CalculateBonusPointPosition(normalisedTargetPosition);

                var rt = target.Value.GetComponent<RectTransform>();
                rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left,bonusPointPosition.x, _blipWidth);
                rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top,bonusPointPosition.y, _blipHeight);
            }
        }
    
        private Vector3 NormalisedPosition(Vector3 playerPos, Vector3 targetPos) {
            var normalisedTargetX = (targetPos.x - playerPos.x)/radarDistance;
            var normalisedTargetZ = (targetPos.z - playerPos.z)/radarDistance;
            
            return new Vector3(normalisedTargetX, 0, normalisedTargetZ);
        }
        
        private Vector2 CalculateBonusPointPosition(Vector3 targetPos) {
            // find the angle from the player to the target.
            var angleToTarget = Mathf.Atan2(targetPos.x,targetPos.z) * Mathf.Rad2Deg;
 
            // The direction the player is facing.
            var anglePlayer = usePlayerDirection? player.eulerAngles.y : 0;
 
            // Subtract the player angle, to get the relative angle to the object. Subtract 90
            // so 0 degrees (the same direction as the player) is Up.
            var angleRadarDegrees = angleToTarget - anglePlayer - 90;
 
            // Calculate the xy position given the angle and the distance.
            var normalisedDistanceToTarget = targetPos.magnitude;
            var angleRadians = angleRadarDegrees * Mathf.Deg2Rad;
            var blipX = normalisedDistanceToTarget * Mathf.Cos(angleRadians);
            var blipY = normalisedDistanceToTarget * Mathf.Sin(angleRadians);
 
            // Scale the blip position according to the radar size.
            blipX *= _radarWidth * .5f;
            blipY *= _radarHeight * .5f;
 
            // Offset the blip position relative to the radar center
            blipX += (_radarWidth * .5f) - _blipWidth * .5f;
            blipY += (_radarHeight * .5f) - _blipHeight * .5f;
 
            return new Vector2(blipX, blipY);
        }

        private void DisplayBonusOnMap(Vector2 pos, RawImage bonusImage)
        {
            var point = Instantiate(bonusImage).gameObject;
            point.transform.SetParent(RadarMapImage.gameObject.transform);
            var rt = point.GetComponent<RectTransform>();
            rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left,pos.x, _blipWidth);
            rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top,pos.y, _blipHeight);
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
        
        protected override void InitManager(InitItemMode mode)
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
            SetValues(new PlayerArgs(PlayerStats.MaxHp, false,  false, false, Vector3.one, 0));
        }

        private void OnRestartButtonClick() => GameEvent.Notify(new CurrentGameArgs(true, false, false, false));
        
        private void OnSaveButtonClick() => GameEvent.Notify(new CurrentGameArgs(false, true, false, false));
        
        private void OnLoadButtonClick() => GameEvent.Notify(new CurrentGameArgs(false, false, true, false));
        
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
        
        public void OnEventRaised(ISubject<BonusManagerArgs> subject, BonusManagerArgs args)
        {
            if (args.ActivatePoints is not null)
            {
                RadarObjectsRepository.UpdateAllWithAction(UnactivateAndDestroyRadarPointAction);
                RadarObjectsRepository.RemoveAll();

                foreach (var point in args.ActivatePoints)
                {
                    var newItem = Instantiate(BonusPointImage);

                    try
                    {
                        newItem.transform.parent = RadarMapImage.transform;
                        RadarObjectsRepository.Insert(point, newItem);
                    }
                    catch (Exception e)
                    {
                        LogException(e);
                    }
                }
            }
            
            if (args.ActivatePoint is not null)
            {
                RadarObjectsRepository.UpdateOnceWithAction(args.ActivatePoint, ActivateRadarPointAction);
            }
            
            if (args.UnactivatePoint is not null)
            {
                RadarObjectsRepository.UpdateOnceWithAction(args.UnactivatePoint, UnactivateRadarPointAction);
            }
            
            if (args.RemoveAll)
            {
                RadarObjectsRepository.UpdateAllWithAction(UnactivateAndDestroyRadarPointAction);
                RadarObjectsRepository.RemoveAll();
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