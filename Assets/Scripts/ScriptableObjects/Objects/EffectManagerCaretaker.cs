using System.Collections.Generic;
using RollABall.Args;
using RollABall.Infrastructure.Memento;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.ScriptableObjects
{
    [CreateAssetMenu(fileName = "EffectManagerCaretaker", menuName = "RollABall/Caretakers/EffectManagerCaretaker", order = 2)]
    public class EffectManagerCaretaker : Caretaker<List<EffectSaveArgs>> { }
}