using System.Collections.Generic;
using RollABall.Args;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    // GameManager + Actions
    public partial class GameManager
    {
        protected override void NewGameAction()
        {
            Caretaker.Init(this, SavedGamePrefix, "SavedGame");
            State = null;
        }

        protected override void SaveGameAction()
        {
            Caretaker.Save();
        }

        protected override void LoadGameAction(SaveGameArgs args)
        {
            // ...
        }

        protected override void RestartGameAction()
        {
            SavedState = new List<ISavableArgs>(new List<SaveGameArgs>());
        }
    }
}