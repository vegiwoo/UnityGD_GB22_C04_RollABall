using System.Collections.Generic;
using RollABall.Args;
using RollABall.Models;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    // GameManager + Actions
    public partial class GameManager
    {
        public override void NewGameAction()
        {
            Caretaker.Init(this, SavedGamePrefix, "SavedGame");
            SavedState = null;
            State = null;
        }

        public override void SaveGameAction()
        {
            Caretaker.Save().Forget();
        }

        public override void LoadGameAction(SaveGameArgs args)
        {
            // ...
        }

        public override void RestartGameAction()
        {
            SavedState = new List<ISavableArgs>(new List<SaveGameArgs>());
        }

        public override void LostGameAction()
        {
           // .. 
        }

        public override void WonGameAction()
        {
            // ... 
        }
    }
}