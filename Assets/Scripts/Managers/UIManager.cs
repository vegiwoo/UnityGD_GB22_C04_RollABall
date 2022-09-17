using System;
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
        
        #endregion
        
        #region MonoBehaviour methods

        private void Start()
        {
            scoreLabel.text = $"score: {0}/{GameStats.GameHighScore}".ToUpper();
        }

        #endregion

        #region Functionality

        public void SetValues(PlayerArgs args)
        {
            hpLabel.text = $"hp: {args.CurrentHp}".ToUpper();
            scoreLabel.text = $"score: {args.GamePoints}/{GameStats.GameHighScore}".ToUpper();
        }
        
        #endregion
    }
}