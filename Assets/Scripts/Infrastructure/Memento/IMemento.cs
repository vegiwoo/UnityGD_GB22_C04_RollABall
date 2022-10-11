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
        /// <remarks>Organizer uses this method to restore its state.</remarks>>
        T State { get; set; }
        
        /// <returns>Used by Caretaker to display metadata.</returns>
        string Name { get; set; }

        /// <returns>Used by Caretaker to display metadata.</returns>
        string Date { get; set; }
    }
}