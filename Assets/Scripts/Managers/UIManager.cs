using System;
using GameDevLib.Interfaces;
using RollABall.Args;
using RollABall.Events;
using RollABall.Stats;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    public class UIManager : MonoBehaviour, GameDevLib.Interfaces.IObserver<PlayerArgs>, IDisposable
    {
        #region Links
        [field:Header("Stats")]
        [field: SerializeField] private GameStats GameStats { get; set; }
        
        [field:Header("Events")]
        [field:SerializeField] private PlayerEvent playerEvent;

        [field:Header("Colors")]
        [SerializeField] private Color32 normalColor = new (236, 217, 21, 255);
        [SerializeField] private Color32 dangerColor = new (236, 107, 22, 255);
        [SerializeField] private Color32 buffColor = new (166, 115, 243, 255);

        #endregion
        
        #region Properties
        [field:Header("Unity events for UI")]
        public UnityEvent<UILabelArgs> hpLabelUpdateEvent;
        public UnityEvent<UILabelArgs> scoreLabelUpdateEvent;
        public UnityEvent<UILabelArgs> speedLabelUpdateEvent;

        #endregion
        

        #region MonoBehaviour methods

        private void OnEnable()
        {
            playerEvent.Attach(this);
        }

        private void OnDisable()
        {
            Dispose();
        }

        private void Start()
        {
            hpLabelUpdateEvent.Invoke(new UILabelArgs("hp", "100", normalColor));
            scoreLabelUpdateEvent.Invoke(new UILabelArgs("score", $"0/{GameStats.GameHighScore}", normalColor));
            speedLabelUpdateEvent.Invoke(new UILabelArgs("speed", "normal", normalColor));
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
            var hpLabelColor = args.CurrentHp < GameStats.CriticalThreshold ? dangerColor : args.IsUnitInvulnerable ? buffColor : normalColor;
            var hpLabelArgs = new UILabelArgs("hp", $"{args.CurrentHp}", hpLabelColor);
            hpLabelUpdateEvent.Invoke(hpLabelArgs);
            
            var scoreLabelColor = args.GamePoints < GameStats.CriticalThreshold  ? dangerColor : normalColor;
            var scoreLabelArgs =
                new UILabelArgs("score", $"{args.GamePoints}/{GameStats.GameHighScore}", scoreLabelColor);
            scoreLabelUpdateEvent.Invoke(scoreLabelArgs);
            
            var speedLabelColor = args.IsSpeedUp == false && args.IsSpeedDown == false ? normalColor :
                args.IsSpeedUp ? buffColor : dangerColor;
            var speedMode = args.IsSpeedUp == false && args.IsSpeedDown == false ? "normal" :
                args.IsSpeedUp ? "high" : "low";
            var speedLabelArgs = new UILabelArgs("speed", speedMode, speedLabelColor);
            speedLabelUpdateEvent.Invoke(speedLabelArgs);
        }
        #endregion
    }
}