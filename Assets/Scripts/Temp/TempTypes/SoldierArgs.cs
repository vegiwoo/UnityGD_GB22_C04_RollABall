using System;

// ReSharper disable once CheckNamespace
namespace RollABall.Test.TempTypes
{
    public class SoldierArgs : EventArgs
    {
        private float Speed { get; }

        public SoldierArgs(float speed)
        {
            Speed = speed;
        }
    }
}