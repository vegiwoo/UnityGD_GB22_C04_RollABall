using System.Collections.Generic;
using System.IO;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Infrastructure.Memento
{
    public abstract class Caretaker<T> : ScriptableObject
    {
        #region Field
        protected List<IMemento<T>> mementos = new ();
        protected string SavedPath { get; set; }
        
        #endregion
    
        #region Properties

        protected IMementoOrganizer<T> Originator { get; set;}
        #endregion
    
        #region Funstionality
        public virtual void Init(IMementoOrganizer<T> originator, string rootPath)
        {
            Originator = originator;
            SavedPath = Path.Combine(rootPath, "Saved/Bonuses") ;
        }

        public abstract void Save();
        public abstract void Load();

        //public void ShowHistory()
        //{
            // Console.WriteLine("Caretaker: Here's the list of mementos:");
            //
            // foreach (var memento in this._mementos)
            // {
            //     Console.WriteLine(memento.GetName());
            // }
        //}

        #endregion
    }
}