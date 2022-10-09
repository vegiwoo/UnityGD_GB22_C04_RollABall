using GameDevLib.Events;
using RollABall.Interactivity.Bonuses;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Events
{
    [CreateAssetMenu(fileName = "ApplyEffectEvent", menuName = "RollABall/Events/ApplyEffectEvent", order = 3)]
    public class ApplyEffectEvent : GameEvent<IEffectable> { }
}