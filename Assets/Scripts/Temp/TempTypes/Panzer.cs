using System;

// ReSharper disable once CheckNamespace
namespace RollABall.Test.TempTypes;

public sealed class Panzer
{
    public event EventHandler<PanzerArgs>? PanzerNotify;

    public Panzer()
    {
        var args = new PanzerArgs(20.0f, 10.0f, 50.0f);
        OnPanzerNotify(args);
    }
    
    private void OnPanzerNotify(PanzerArgs e)
    {
        PanzerNotify?.Invoke(this, e);
    }
}