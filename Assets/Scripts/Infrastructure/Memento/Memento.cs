using System;

// ReSharper disable once CheckNamespace
namespace RollABall.Infrastructure.Memento
{
    /// <summary>
    /// Represents an abstract memento in the 'Memento' pattern.
    /// </summary>
    /// <remarks>Used in 'Memento' pattern.</remarks>>
    public abstract class Memento<T> : IMemento<T>
    {
        protected T State { get; set; }
        private readonly DateTime _date = DateTime.Now;
        
        
        public virtual string GetName() => $"{_date} - {nameof(Memento<T>)}";

        public virtual T GetState() => State;

        public virtual DateTime GetDate() => _date;
    }
}