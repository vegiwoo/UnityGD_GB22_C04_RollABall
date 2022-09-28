using GameDevLib.Events;
using RollABall.Args;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Events
{
    [CreateAssetMenu(fileName = "CurrentGameEvent", menuName = "RollABall/Events/CurrentGameEvent", order = 1)]
    public class CurrentGameEvent : GameEvent<CurrentGameArgs> { }
}