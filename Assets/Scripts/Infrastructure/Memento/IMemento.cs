using System;

// ReSharper disable once CheckNamespace
namespace RollABall.Infrastructure.Memento
{
    /// <summary>
    /// Organizer snapshot interface.
    /// </summary>
    /// <remarks>Used in 'Memento' pattern.</remarks>>
    public interface IMemento<T>
    {
        /// <returns>Used by Caretaker to display metadata.</returns>
        string GetName();

        /// <remarks>Organizer uses this method to restore its state.</remarks>>
        T GetState();

        /// <returns>Used by Caretaker to display metadata.</returns>
        DateTime GetDate();
    }
}