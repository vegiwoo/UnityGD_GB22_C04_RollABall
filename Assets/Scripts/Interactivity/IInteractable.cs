
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

        /// <summary>
        /// Interactive Event Trigger.
        /// </summary>
        /// <param name="element">Interaction element</param>
        /// <param name="tag">Tag received from interaction element</param>
        void OnGettingNotify(T element, string tag);
    }
}