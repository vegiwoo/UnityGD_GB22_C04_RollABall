using GameDevLib.Events;
using RollABall.Args;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Events
{
    [CreateAssetMenu(fileName = "BonusEvent", menuName = "RollABall/Events/BonusEvent", order = 2)]
    public class BonusEvent : GameEvent<BonusArgs> { }
}
