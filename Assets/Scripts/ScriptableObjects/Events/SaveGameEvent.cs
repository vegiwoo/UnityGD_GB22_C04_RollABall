using System.Collections.Generic;
using GameDevLib.Events;
using RollABall.Args;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Events
{
    [CreateAssetMenu(fileName = "SaveGameEvent", menuName = "RollABall/Events/SaveGameEvent", order = 6)]
    public class SaveGameEvent : GameEvent<IList<ISavableArgs>> { }
}