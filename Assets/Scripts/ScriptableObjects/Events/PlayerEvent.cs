using GameDevLib.Events;
using RollABall.Args;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Events
{
    [CreateAssetMenu(fileName = "PlayerEvent", menuName = "RollABall/Events/PlayerEvent", order = 0)]
    public class PlayerEvent : GameEvent<PlayerArgs> { }
}