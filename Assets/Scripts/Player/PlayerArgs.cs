#nullable enable
using RollABall.Interactivity.Bonuses;

// ReSharper disable once CheckNamespace
namespace RollABall.Args
{
    public struct PlayerArgs
    {
        public float CurrentHp { get; }

        public PlayerArgs(float currentHp)
        {
            CurrentHp = currentHp;
        }
    }
}