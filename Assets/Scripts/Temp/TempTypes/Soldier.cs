using System;

// ReSharper disable once CheckNamespace
namespace RollABall.Test.TempTypes
{
    public sealed class Soldier
    {
        public event EventHandler<SoldierArgs>? SoldierNotify;

        public Soldier()
        {
            var args = new SoldierArgs(0.5f);
            OnSoldierNotify(args);
        }

        private void OnSoldierNotify(SoldierArgs e)
        {
            SoldierNotify?.Invoke(this, e);
        }
    }
}