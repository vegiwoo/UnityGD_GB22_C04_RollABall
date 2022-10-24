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
        protected override void NewGameAction()
        {
            _activeEffectsByTarget = new Dictionary<EffectTargetType, Coroutine>();
            State = new List<ISavableArgs>(new List<EffectSaveArgs>());
        }
        
        protected override void SaveGameAction()
        {
            // TODO: Notify with state
        }

        protected override void LoadGameAction(SaveGameArgs args)
        {
            StopAllCoroutines();
            
            _activeEffectsByTarget.Clear();

            if (args.EffectSaveArgs is null) return;
            
            State = new List<ISavableArgs>(args.EffectSaveArgs);
                
            foreach (var item in State.OfType<EffectSaveArgs>())
            {
                ApplyEffectOnPlayer(item.AppliedEffect, item.RemainingDuration);
            }
        }

        protected override void RestartGameAction()
        {
            StopAllCoroutines();
            
            _activeEffectsByTarget = new Dictionary<EffectTargetType, Coroutine>();
            State = new List<ISavableArgs>(new List<EffectSaveArgs>());
        }
    }
}