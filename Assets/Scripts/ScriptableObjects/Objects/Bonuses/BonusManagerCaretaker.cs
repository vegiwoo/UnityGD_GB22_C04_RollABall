#nullable enable

using System.Collections.Generic;
using RollABall.Infrastructure.Memento;
using RollABall.Interactivity.Bonuses;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.ScriptableObjects
{
    [CreateAssetMenu(fileName = "BonusManagerCaretaker", menuName = "RollABall/Caretakers/BonusManagerCaretaker",
        order = 0)]
    public class BonusManagerCaretaker : Caretaker<List<BonusManagerStateItem>> { }
}