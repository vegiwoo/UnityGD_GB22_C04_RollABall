using System;

// ReSharper disable once CheckNamespace
namespace RollABall.Test.TempTypes;

public class PanzerArgs : EventArgs
{
    private float Speed { get; }
    private float ArmorPower { get; }
    private float DestructiveForce { get;  }

    public PanzerArgs(float speed, float armorPower, float destructiveForce)
    {
        Speed = speed;
        ArmorPower = armorPower;
        DestructiveForce = destructiveForce;
    }
}
