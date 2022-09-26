using System.Collections.Generic;
using GameDevLib.Interfaces;
using RollABall.Args;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Events
{
    [CreateAssetMenu(fileName = "EffectEvent", menuName = "RollABall/Events/EffectEvent", order = 2)]
    public class EffectEvent : ScriptableObject, ISubject<EffectArgs>
    {
        private readonly List<IObserver<EffectArgs>> _observers = new ();
        
        public void Attach(IObserver<EffectArgs> observer)
        {
            if (!_observers.Contains(observer))  _observers.Add(observer);
        }

        public void Detach(IObserver<EffectArgs> observer)
        {
            if (_observers.Contains(observer))  _observers.Remove(observer);
        }

        public void Notify(EffectArgs args)
        {
            foreach (var observer in _observers)
            {
                observer.OnEventRaised(this, args);
            }
        }
    }
}