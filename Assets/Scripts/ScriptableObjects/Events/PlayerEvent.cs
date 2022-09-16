using System.Collections.Generic;
using RollABall.Args;
using GameDevLib.Interfaces;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Events
{

    
    [CreateAssetMenu(fileName = "PlayerEvent", menuName = "RollABall/Events/PlayerEvent", order = 0)]
    public class PlayerEvent : ScriptableObject, ISubject<PlayerArgs>
    {
        private readonly List<IObserver<PlayerArgs>> _observers = new ();
        
        public void Attach(IObserver<PlayerArgs> observer)
        {
            if (!_observers.Contains(observer))  _observers.Add(observer);
        }

        public void Detach(IObserver<PlayerArgs> observer)
        {
            if (_observers.Contains(observer))  _observers.Remove(observer);
        }

        public void Notify(PlayerArgs args)
        {
            foreach (var observer in _observers)
            {
                observer.OnEventRaised(this, args);
            }
        }
    }
}


