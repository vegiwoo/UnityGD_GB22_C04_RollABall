using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity
{
    /// <summary>
    /// Base class for all objects that a character can interact with.
    /// </summary>
    public abstract class InteractiveObject : MonoBehaviour
    {
        protected abstract void Interaction();
    }
}