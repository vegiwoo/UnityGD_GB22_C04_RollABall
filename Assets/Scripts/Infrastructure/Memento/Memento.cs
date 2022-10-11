using System;
using System.Globalization;

// ReSharper disable once CheckNamespace
namespace RollABall.Infrastructure.Memento
{
    /// <summary>
    /// Represents an abstract memento in the 'Memento' pattern.
    /// </summary>
    /// <remarks>Used in 'Memento' pattern.</remarks>>
    [Serializable]
    public abstract class Memento<T> : IMemento<T>
    {
        #region Properties
        
        public T State { get; set; }
        public virtual string Name => $"{nameof(Memento<T>)}_{Date}";
        public string Date => DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:sszz");
        
        #endregion
        
        #region Functionality
        
        public override string ToString()
        {   
            return $"{Name}, {Date}, {State.ToString()}";
        }
        
        #endregion
    }
}