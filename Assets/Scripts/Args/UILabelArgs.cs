using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Args
{
    public class UILabelArgs : EventArgs
    {
        public string Prefix { get; }
        public string Suffix { get; }
        public Color Color { get; }

        public UILabelArgs(string prefix, string suffix, Color color)
        {
            Prefix = prefix;
            Suffix = suffix;
            Color = color;
        }
    }
}