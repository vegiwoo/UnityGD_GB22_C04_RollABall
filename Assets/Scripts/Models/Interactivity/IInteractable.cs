
// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity
{
    public interface IInteractable<T>
    {
        /// <summary>
        /// Interactive event delegate.
        /// </summary>
        delegate void InteractiveHandler(T element, string tag);

        /// <summary>
        /// interactive event.
        /// </summary>
        event InteractiveHandler InteractiveNotify;
    }
}