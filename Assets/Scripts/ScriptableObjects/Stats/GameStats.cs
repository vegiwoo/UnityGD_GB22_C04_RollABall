using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Stats
{
    [CreateAssetMenu(fileName = "GameStats", menuName = "RollABall/Stats/GameStats", order = 2)]
    public class GameStats : ScriptableObject
    {
        [field: SerializeField] public int GameHighScore { get; set; } = 100;
    }
}

