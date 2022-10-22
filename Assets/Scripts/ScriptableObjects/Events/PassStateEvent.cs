using System;
using System.Collections.Generic;
using GameDevLib.Events;
using RollABall.Args;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Events
{
    [CreateAssetMenu(fileName = "PassStateEvent", menuName = "RollABall/Events/PassStateEvent", order = 6)]
    public class PassStateEvent : GameEvent<List<ISavableArgs>> { }
}