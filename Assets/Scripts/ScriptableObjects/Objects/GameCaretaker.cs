using System.Collections.Generic;
using RollABall.Args;
using RollABall.Infrastructure.Memento;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.ScriptableObjects
{
    [CreateAssetMenu(fileName = "GameCaretaker", menuName = "RollABall/Objects/GameCaretaker", order = 3)]
    public class GameCaretaker : Caretaker<List<ISavableArgs>> { }
}