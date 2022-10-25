using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using RollABall.Args;
using RollABall.Infrastructure.Memento;
using Cysharp.Threading.Tasks;
using GameDevLib.Interfaces;
using RollABall.Events;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    // Work with memento pattern
    public partial class GameManager : IMementoOrganizer<SaveGameArgs>, GameDevLib.Interfaces.IObserver<IList<ISavableArgs>>
    {
        [field: Header("Memento")] 
        [field: SerializeField] private SaveGameEvent SaveGameEvent { get; set; }
        public SaveGameArgs State { get; set; }

        private const string SavedGamePrefix = "SavedGame_";
        
        public async UniTask<IMemento<SaveGameArgs>> Save()
        {
            State = new SaveGameArgs();
            
            await UniTask.WaitUntilValueChanged(this, m => m.State.PlayerArgs != null);
            await UniTask.WaitUntilValueChanged(this, m => m.State.BonusSaveArgs != null);
            await UniTask.WaitUntilValueChanged(this, m => m.State.EffectSaveArgs != null);

            Debug.Log("Yep!");
            
            return new Memento<SaveGameArgs>(State, SavedGamePrefix);
        }
        
        public void Load(IMemento<SaveGameArgs> memento)
        {
            //throw new System.NotImplementedException();
        }

        // SaveGameEvent handling
        public void OnEventRaised(ISubject<IList<ISavableArgs>> subject, IList<ISavableArgs> args)
        {
            Debug.Log(args.GetType());
        }
    }
}

// public IMemento<PlayerArgs> Save()
// {
//     State = MakeState();
//     return new Memento<PlayerArgs>(State, "Player");
// }
//
// public void Load(IMemento<PlayerArgs> memento)
// {
//     if (memento is not Memento<PlayerArgs>)
//     {
//         throw new Exception("Unknown memento class " + memento.ToString());
//     }
//     
//     State = memento.State;
//     InitPlayer(true);
// }
        
// // Memento pattern - init caretaker for organizer.
// Caretaker.Init(this, "Player", "PlayerMemento");