using GameDevLib.Events;
using RollABall.Args;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Events
{
    [CreateAssetMenu(fileName = "EffectEvent", menuName = "RollABall/Events/EffectEvent", order = 3)]
    public class EffectEvent : GameEvent<EffectArgs> { }
}