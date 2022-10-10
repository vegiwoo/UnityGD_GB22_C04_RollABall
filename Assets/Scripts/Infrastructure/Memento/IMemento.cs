using System;

// ReSharper disable once CheckNamespace
namespace RollABall.Infrastructure.Memento
{
    /// <summary>
    /// Represents interface for a snapshot in 'Memento' pattern.
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