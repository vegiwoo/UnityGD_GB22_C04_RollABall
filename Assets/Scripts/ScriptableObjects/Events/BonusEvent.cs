using System.Collections.Generic;
using GameDevLib.Interfaces;
using RollABall.Args;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Events
{
    [CreateAssetMenu(fileName = "BonusEvent", menuName = "RollABall/Events/BonusEvent", order = 1)]
    public class BonusEvent : ScriptableObject, ISubject<BonusArgs>
    {
        private readonly List<IObserver<BonusArgs>> _observers = new ();
        
        public void Attach(IObserver<BonusArgs> observer)
        {
            if (!_observers.Contains(observer))  _observers.Add(observer);
        }

        public void Detach(IObserver<BonusArgs> observer)
        {
            if (_observers.Contains(observer))  _observers.Remove(observer);
        }

        public void Notify(BonusArgs args)
        {
            foreach (var observer in _observers)
            {
                observer.OnEventRaised(this, args);
            }
        }
    }
}
