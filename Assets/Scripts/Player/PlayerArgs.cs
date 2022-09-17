#nullable enable

// ReSharper disable once CheckNamespace
namespace RollABall.Args
{
    public struct PlayerArgs
    {
        public float CurrentHp { get; }

        public int GamePoints { get; }
        public PlayerArgs(float currentHp, int gamePoints = 0)
        {
            CurrentHp = currentHp;
            GamePoints = gamePoints;
        }
    }
}