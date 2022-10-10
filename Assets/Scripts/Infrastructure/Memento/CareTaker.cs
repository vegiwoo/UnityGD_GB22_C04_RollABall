using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Infrastructure.Memento
{
    public abstract class Caretaker<T> : ScriptableObject
    {
        #region Field
        protected List<IMemento<T>> mementos = new ();
        private string SavedPath { get; set; }
        
        #endregion
    
        #region Properties

        private IMementoOrganizer<T> Originator { get; set;}
        #endregion
    
        #region Funstionality
        public void Init(IMementoOrganizer<T> originator, string rootPath)
        {
            Originator = originator;
            SavedPath = Path.Combine(rootPath, "Saved/Bonuses") ;
            
            // Достать все предыдущие сохрания, обновить mementos !!! 
        }
    
        public void Save()
        {
            var memento = Originator.Save();

            var serializer = new JsonSerializer
            {
                //serializer.Converters.Add(new UnixDateTimeConverter());
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };

            
            // Check Directory
            if (!Directory.Exists(SavedPath))
            {
                Directory.CreateDirectory(SavedPath);
            }
            
            var savedPath = Path.Combine(SavedPath, "testBonuses.txt");
            //Debug.Log(savedPath);
            
            using (var streamWriter = new StreamWriter(savedPath))
            {
                var state = memento.GetState();
                Debug.Log(state);
                
                var writer = new JsonTextWriter(streamWriter);
                serializer.Serialize(writer, memento.GetState());
                Debug.Log("Yep!");
            }
            
            // using var streamWriter = new StreamWriter(savedPath);
            // using var writer = new JsonTextWriter(streamWriter);
            // serializer.Serialize(writer, memento);
            // Debug.Log("Yep!");

            //mementos.Add(Originator.Save());
        }
        
        public void Load()
        {
            // if (this._mementos.Count == 0)
            // {
            //     return;
            // }
            //
            // var memento = this._mementos.Last();
            // this._mementos.Remove(memento);
            //
            // Console.WriteLine("Caretaker: Restoring state to: " + memento.GetName());
            //
            // try
            // {
            //     this._originator.Restore(memento);
            // }
            // catch (Exception)
            // {
            //     this.Undo();
            // }
        }

        public void ShowHistory()
        {
            // Console.WriteLine("Caretaker: Here's the list of mementos:");
            //
            // foreach (var memento in this._mementos)
            // {
            //     Console.WriteLine(memento.GetName());
            // }
        }

        #endregion
    }
}