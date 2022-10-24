using System.Collections.Generic;
using RollABall.Args;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    // GameManager + Actions
    public partial class GameManager
    {
        protected override void NewGameAction()
        {
            State = new List<ISavableArgs>(new List<SaveGameArgs>());
        }

        protected override void SaveGameAction()
        {
            // ...
        }

        protected override void LoadGameAction(SaveGameArgs args)
        {
            // ...
        }

        protected override void RestartGameAction()
        {
            State = new List<ISavableArgs>(new List<SaveGameArgs>());
        }
    }
}