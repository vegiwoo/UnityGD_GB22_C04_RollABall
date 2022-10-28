#nullable enable

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using RollABall.Interactivity.Bonuses;
using RollABall.Managers;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Args
{
    public class ActiveEffectArg : IDisposable
    {
        public IEffectable Effect { get; }
        private CancellationTokenSource CancellationTokenSource { get;  }

        public float RemainingDuration { get; set; }

        public ActiveEffectArg(IEffectable effect, CancellationTokenSource cancellationTokenSource, float remainingDuration)
        {
            Effect = effect;
            CancellationTokenSource = cancellationTokenSource;
            RemainingDuration = remainingDuration;
        }

        public EffectSaveArgs SaveEffect()
        {
            return new EffectSaveArgs((Effect)Effect, RemainingDuration);
        }

        public void Dispose()
        {
            CancellationTokenSource.Cancel();
            CancellationTokenSource.Dispose();
        }
    }
}

