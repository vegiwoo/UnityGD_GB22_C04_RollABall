using RollABall.Args;
using RollABall.Infrastructure.Memento;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.ScriptableObjects
{
    [CreateAssetMenu(fileName = "PlayerCaretaker", menuName = "RollABall/Caretakers/PlayerCaretaker", order = 1)]
    public class PlayerCaretaker : Caretaker<PlayerArgs> { }
}