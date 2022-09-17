using GameDevLib.Interfaces;
using RollABall.Args;
using RollABall.Events;
using RollABall.Stats;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Debug;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    public class GameManager : MonoBehaviour, GameDevLib.Interfaces.IObserver<PlayerArgs>
    {
        #region Links
        [field:Header("Links")]
        [field: SerializeField] private BonusManager BonusManager { get; set; }
        [field: SerializeField] private UIManager UIManager { get; set; }
        [field:Header("Stats")]
        [field: SerializeField] private GameStats GameStats { get; set; }
        [field:Header("Events")]
        [field:SerializeField] private PlayerEvent playerEvent;
        #endregion
        
        #region Properties
        private int GameCurrentScore { get; set; }
        #endregion
        
        #region MonoBehaviour methods

        private void Start()
        {
            GameCurrentScore = 0;
        }

        private void OnEnable()
        {
            playerEvent.Attach(this);
        }

        private void OnDisable()
        {
            playerEvent.Detach(this);
        }

        #endregion
        
        #region Functionality
        
        public void OnEventRaised(ISubject<PlayerArgs> subject, PlayerArgs args)
        {
            UIManager.SetValues(args);

            var lost = args.CurrentHp <= 0;
            var win = args.GamePoints >= GameStats.GameHighScore;
            
            EditorApplication.isPaused = lost|| win;
            if (lost)
            {
                Log("You lose :(");
            } else if (win)
            {
                Log("You win :)");
            }
        }
        
        #endregion
    }
}