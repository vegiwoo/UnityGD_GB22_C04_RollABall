using System;
using System.Collections.Generic;
using System.Linq;
using RollABall.Args;
using RollABall.Interactivity.Bonuses;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    public partial class EffectManager
    {
        public override void NewGameAction()
        {
           _activeEffectsOnPlayer = new List<ActiveEffectArg>();
        }
        
        public override void SaveGameAction()
        {
            SavedState = new List<ISavableArgs>(new List<EffectSaveArgs>());
            foreach (var value in _activeEffectsOnPlayer)
            {
                SavedState.Add(value.SaveEffect());
            }
            SaveGameEvent.Notify(SavedState);
        }

        public override void LoadGameAction(SaveGameArgs args)
        {
            StopAllCoroutines();
            _activeEffectsOnPlayer.Clear();

            SavedState = new List<ISavableArgs>(args.EffectSaveArgs);
                
            foreach (var item in SavedState.OfType<EffectSaveArgs>())
            {
                ApplyEffectOnPlayer(item.AppliedEffect, item.RemainingDuration);
            }
        }

        public override void RestartGameAction()
        {
            StopAllCoroutines();
            NewGameAction();
        }

        public override void LostGameAction()
        {
            StopAllCoroutines();
            _activeEffectsOnPlayer.Clear();
        }

        public override void WonGameAction()
        {
            StopAllCoroutines();
            _activeEffectsOnPlayer.Clear();
        }
    }
}