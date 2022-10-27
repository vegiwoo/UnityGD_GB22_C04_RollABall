using System.Collections.Generic;
using GameDevLib.Interfaces;
using RollABall.Args;
using RollABall.Events;
using RollABall.Models;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Player
{
    public partial class PlayerBall : IObserver<CurrentGameArgs>
    {
        #region Links
        
        [field: Header("Memento")] 
        [field: SerializeField] private SaveGameEvent SaveGameEvent { get; set; }
        
        #endregion
        
        #region Properties 
        
        public override List<ISavableArgs> SavedState { get; set; }
        
        #endregion
        
        #region Functionality
        
        public override void NewGameAction()
        {
            GamePoints = 0;
            CurrentHp = PlayerStats.MaxHp;
            IsUnitInvulnerable = false;
            SpeedMultiplier = SpeedMultiplierConst;

            transform.position = GameStats.PlayerSpawnPoint;
            playerRb.velocity = Vector3.zero;
        }

        public override void SaveGameAction()
        {
            ResetSavedState();
            
            var currentState = MakeState();
            SavedState.Add(currentState);
            SaveGameEvent.Notify(SavedState);
        }

        public override void LoadGameAction(SaveGameArgs args)
        {
            ResetSavedState();
            
            SavedState.Add(args.PlayerArgs);
            if (SavedState[0] is not PlayerArgs currentState) return;
            
            GamePoints = currentState.GamePoints;
            CurrentHp = currentState.CurrentHp;
            IsUnitInvulnerable = currentState.IsUnitInvulnerable;
            SpeedMultiplier = SpeedMultiplierConst;

            transform.position = new Vector3(currentState.Point.PosX, currentState.Point.PosY, currentState.Point.PosZ);
            playerRb.velocity = Vector3.zero;
        }

        public override void RestartGameAction()
        {
            NewGameAction();
        }
        
        public override void LostGameAction()
        {
            // ... 
        }

        public override void WonGameAction()
        {
            // ... 
        }
        
        public void ResetSavedState()
        {
            if (SavedState is null)
            {
                SavedState = new List<ISavableArgs>(new List<PlayerArgs>());
            }
            else
            {
                SavedState.Clear();
            }
        }

        public void OnEventRaised(ISubject<CurrentGameArgs> subject, CurrentGameArgs args)
        {
            if (args.CurrentGameState.HasValue)
            {
                switch (args.CurrentGameState.Value)
                {
                    case CurrentGameState.Restart:
                        RestartGameAction();
                        break;
                    case CurrentGameState.Save:
                        SaveGameAction();
                        break;
                    case CurrentGameState.Load when args.SaveGameArgs is not null:
                        LoadGameAction(args.SaveGameArgs);
                        break;
                    case CurrentGameState.Lost:
                        LostGameAction();
                        break;
                    case CurrentGameState.Won:
                        WonGameAction();
                        break;
                }
            }
        }
        
        #endregion
    }
}