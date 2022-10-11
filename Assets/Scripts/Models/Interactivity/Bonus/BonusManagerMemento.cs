using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using RollABall.Infrastructure.Memento;

// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Bonuses
{
    [Serializable]
    public class BonusManagerMemento : IMemento<List<BonusManagerStateItem>>
    {
        #region Properties
        
        public string Name { get; set; }
        public string Date { get; set; }
        public List<BonusManagerStateItem> State { get; set; }
        
        #endregion
        
        #region Constructors
        
        [JsonConstructor]
        public BonusManagerMemento(string name, string date, List<BonusManagerStateItem> state)
        {
            Name = name;
            Date = date;
            State = state;
        }

        public BonusManagerMemento(List<BonusManagerStateItem> state)
        {
            Date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:sszz");
            Name = $"BonusMemento_{Date}";
            State = state;
        }
        
        #endregion
        
        #region Finctionality
        
        public override string ToString()
        {   
            return $"{Name}, {Date}, {State}";
        }
        
        #endregion
    }
}