using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Infrastructure.Memento
{
    public abstract class Caretaker<T,D> : ScriptableObject
    {
        #region Field
        // When saving multiple snapshots, a collection is used:
        //protected readonly List<(IMemento<T> memento, string path)> mementos = new ();
        protected (IMemento<T> memento, string path)? savedMemento;

        protected readonly JsonSerializerSettings jsonFormatSetting = new ()
        {
            Formatting = Formatting.Indented,
            DateFormatHandling = DateFormatHandling.IsoDateFormat
        };
        
        #endregion
    
        #region Properties

        protected IMementoOrganizer<T> Originator { get; set;}
        protected string SavedPath { get; set; }
        protected string NamePrefix { get; set; }
        private static string SavedDir => "Saved";
        
        #endregion
    
        #region Functionality
        public void Init(IMementoOrganizer<T> originator, string dirName, string namePrefix)
        {
            Originator = originator;
            SavedPath = Path.Combine(Application.persistentDataPath, $"{SavedDir}/{dirName}/");
            NamePrefix = namePrefix;

            savedMemento = null;
            Preload();
        }

        public void Save()
        {
            // Get memento
            var memento = Originator.Save();
            
            // Check Directory
            var path = CheckExistenceDirectory(SavedPath, true);
            if (path is null)
            {
                throw new ArgumentException("No path to saved.");
            }
            
            var savedPath = Path.Combine(path, $"{memento.Name}.json");

            // Writing 
            using var writer = new StreamWriter(savedPath, false, Encoding.Default);
            var json = JsonConvert.SerializeObject(memento, jsonFormatSetting);
            writer.Write(json);
            
            // Save memento and path
            savedMemento = (memento, savedPath);
                
            Debug.Log($"Bonuses Saved ({memento.Name})");
        }
        
        protected abstract void Preload();
        
        public void Load()
        {
            if(savedMemento is not null)
            {
                Originator.Load(savedMemento.Value.memento);
                Debug.Log($"Bonuses Loaded ({savedMemento.Value.memento.Name})");
            }
            else
            {
                throw new ArgumentException("No memento to load.");
            }
        }
        
        /// <summary>
        /// Checks for existence of a directory at specified path.
        /// </summary>
        /// <param name="savedPath">Path to check directory.</param>
        /// <param name="create">Flag for creating a directory if it is not found.</param>
        /// <returns>Path to an existing directory (optional).</returns>
        protected static string? CheckExistenceDirectory(string savedPath, bool create)
        {
            while (true)
            {
                if (!Directory.Exists(savedPath))
                {
                    if (create)
                    {
                        Directory.CreateDirectory(savedPath);
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return savedPath;
                }
            }
        }

        #endregion
    }
}