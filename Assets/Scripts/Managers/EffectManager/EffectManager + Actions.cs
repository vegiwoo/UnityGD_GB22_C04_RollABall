using System.Collections.Generic;
using System.Linq;
using RollABall.Args;
using RollABall.Interactivity.Effects;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    public partial class EffectManager
    {
        public override void NewGameAction()
        {
            _activeEffectsByTarget = new Dictionary<EffectTargetType, Coroutine>();
            SavedState = new List<ISavableArgs>(new List<EffectSaveArgs>());
        }
        
        public override void SaveGameAction()
        {
            // TODO: Notify with state
        }

        public override void LoadGameAction(SaveGameArgs args)
        {
            StopAllCoroutines();
            _activeEffectsByTarget.Clear();

            if (args.EffectSaveArgs is null) return;
            
            SavedState = new List<ISavableArgs>(args.EffectSaveArgs);
                
            foreach (var item in SavedState.OfType<EffectSaveArgs>())
            {
                ApplyEffectOnPlayer(item.AppliedEffect, item.RemainingDuration);
            }
        }

        public override void RestartGameAction()
        {
            StopAllCoroutines();
            
            _activeEffectsByTarget = new Dictionary<EffectTargetType, Coroutine>();
            SavedState = new List<ISavableArgs>(new List<EffectSaveArgs>());
        }

        public override void LostGameAction()
        {
            StopAllCoroutines();
            _activeEffectsByTarget.Clear();
        }

        public override void WonGameAction()
        {
            StopAllCoroutines();
            _activeEffectsByTarget.Clear();
        }
    }
}