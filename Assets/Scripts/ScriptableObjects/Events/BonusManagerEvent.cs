using GameDevLib.Events;
using RollABall.Args;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Events
{
    [CreateAssetMenu(fileName = "BonusManagerEvent", menuName = "RollABall/Events/BonusManagerEvent", order = 5)]
    public class BonusManagerEvent : GameEvent<BonusManagerArgs> { }
}

