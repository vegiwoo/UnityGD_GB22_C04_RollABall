using System.IO;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Infrastructure.Memento
{
    public abstract class Caretaker<T> : ScriptableObject
    {
        #region Field
        // When saving multiple snapshots, a collection is used:
        //protected readonly List<(IMemento<T> memento, string path)> mementos = new ();
        protected (IMemento<T> memento, string path)? savedMemento;
        protected string SavedPath { get; set; }
        protected string NamePrefix { get; set; }
        private static string SavedDir => "Saved";
        
        #endregion
    
        #region Properties

        protected IMementoOrganizer<T> Originator { get; set;}
        #endregion
    
        #region Funstionality
        public void Init(IMementoOrganizer<T> originator, string dirName, string namePrefix)
        {
            Originator = originator;
            SavedPath = Path.Combine(Application.persistentDataPath, $"{SavedDir}/{dirName}/");
            NamePrefix = namePrefix;

            savedMemento = null;
            Preload();
        }

        public abstract void Save();

        protected abstract void Preload();
        public abstract void Load();

        #endregion
    }
}