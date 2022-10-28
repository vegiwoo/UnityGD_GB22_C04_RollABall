using System;
using System.Collections.Generic;
using System.Linq;
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
            var playerArgsTask =  UniTask.WaitUntil(() => State.PlayerArgs is not null);
            var bonusesArgsTask =  UniTask.WaitUntil(() => State.BonusSaveArgs is not null);
            var effectsArgsTask =  UniTask.WaitUntil(() => State.EffectSaveArgs is not null);
            
            await (playerArgsTask, bonusesArgsTask, effectsArgsTask);

            return new Memento<SaveGameArgs>(State, SavedGamePrefix);
        }
        
        public void Load(IMemento<SaveGameArgs> memento)
        {
            // ... 
        }
        
        public void OnEventRaised(ISubject<IList<ISavableArgs>> subject, IList<ISavableArgs> args)
        {
            void IsStateExist()
            {
                State ??= new SaveGameArgs();
            }

            try
            {
                var playerArgs = args.Cast<PlayerArgs>().First();
                IsStateExist();
                State.PlayerArgs = playerArgs;
            }
            catch (Exception e)
            {
                // ignores
            }
            
            try
            {
                var bonusArgs = args.Cast<BonusSaveArgs>().ToList();
                IsStateExist();
                State.BonusSaveArgs = bonusArgs;
            }
            catch (Exception e)
            {
                // ignores
            }
            
            try
            {
                var effectArgs = args.Cast<EffectSaveArgs>().ToList();
                IsStateExist();
                State.EffectSaveArgs = effectArgs;
                Debug.Log("Effects ok");
            }
            catch (Exception e)
            {
                // ignores
            }
        }
    }
}