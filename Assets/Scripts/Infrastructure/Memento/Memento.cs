using System;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace RollABall.Infrastructure.Memento
{
    [Serializable]
    public class Memento<T> : IMemento<T>
    {
        #region Properties 
    
        public T State { get; }
        public string Name { get; }
        public string Date { get; set; }
    
        #endregion

        #region Constructors
    
        [JsonConstructor]
        public Memento(T state, string name, string date)
        {
            State = state;
            Name = name;
            Date = date;
        }

        public Memento(T state, string prefix)
        {
            State = state;
            Date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:sszz");
            Name = $"{prefix}_{Date}";
        }

        #endregion
    
        #region Functionality
    
        public override string ToString()
        {   
            return $"{Name}, {Date}, {State}";
        }
    
        #endregion
    }
}