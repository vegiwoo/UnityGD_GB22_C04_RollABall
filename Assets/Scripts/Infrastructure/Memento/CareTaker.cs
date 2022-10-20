using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Infrastructure.Memento
{
    /// <summary>
    /// Caretaker to save, store and download mementos.
    /// </summary>
    /// <typeparam name="TState">Type of State parameter in IMemento<T>.</typeparam>
    public abstract class Caretaker<TState> : ScriptableObject
    {
        #region Field

        // When saving multiple snapshots, a collection is used:
        // protected readonly List<(IMemento<T> memento, string path)> mementos = new ();
        private (IMemento<TState> memento, string path)? _savedMemento;

        private readonly JsonSerializerSettings _jsonFormatSetting = new ()
        {
            Formatting = Formatting.Indented,
            DateFormatHandling = DateFormatHandling.IsoDateFormat
        };
        
        private string SavedEntityName { get; set; }
        
        #endregion
    
        #region Properties

        private IMementoOrganizer<TState> Originator { get; set;}
        private string SavedPath { get; set; }
        private string NamePrefix { get; set; }
        private static string SavedDir => "Saved";
        
        #endregion

        #region Functionality
        public void Init(IMementoOrganizer<TState> originator, string dirName, string namePrefix)
        {
            Originator = originator;
            SavedPath = Path.Combine(Application.persistentDataPath, $"{SavedDir}/{dirName}/");
            NamePrefix = namePrefix;
            SavedEntityName = dirName;
            
            _savedMemento = null;
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
            var json = JsonConvert.SerializeObject(memento, _jsonFormatSetting);
            writer.Write(json);
            
            // Save memento and path
            _savedMemento = (memento, savedPath);
                
            Debug.Log($"{SavedEntityName} saved ({memento.Name})");
        }

        private void Preload()
        {
            Action<string> debugLogMessage = Debug.Log;
            
            var dir = CheckExistenceDirectory(SavedPath, false);
            if (dir is null)
            {
                debugLogMessage($"No saved ({SavedEntityName}) games.");
                return;
            }
            
            var files = Directory.GetFiles(dir).Where(n => n.Contains(NamePrefix)).ToArray();
            if (files.Length > 0)
            {
                var oneFilePath = "";
                    
                // Cleaning a directory from other files
                var filterFiles = files.Where(n => n.Contains(NamePrefix)).ToArray();
                foreach (var filePath in filterFiles)
                {
                    if (filePath == filterFiles.First())
                    {
                        oneFilePath = filePath;
                    }
                    else
                    {
                        File.Delete(filePath); 
                    }
                }  
                    
                // Reading 
                using var reader = new StreamReader(oneFilePath);
                var jsonString = reader.ReadToEnd();
                var memento = JsonConvert.DeserializeObject<Memento<TState>>(jsonString, _jsonFormatSetting);

                // Saved in savedMemento
                if (memento is not null)
                {
                    _savedMemento = (memento, oneFilePath);
                    debugLogMessage($"Game save ({SavedEntityName}) preloaded.");
                }
                else
                {
                    debugLogMessage($"No saved ({SavedEntityName}) games.");
                }
            }
            else
            {
                debugLogMessage($"No saved ({SavedEntityName}) games.");
            }
        }
        
        public void Load()
        {
            if(_savedMemento is not null)
            {
                Originator.Load(_savedMemento.Value.memento);
                Debug.Log($"{SavedEntityName} Loaded ({_savedMemento.Value.memento.Name})");
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
        private static string? CheckExistenceDirectory(string savedPath, bool create)
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
        
        /// <summary>
        /// Gets collection of types that generic interface is typed with. 
        /// </summary>
        /// <param name="typeImplements">Type that implements interface.</param>
        /// <param name="typeGenericInterface">Type of generic interface.</param>
        /// <returns></returns>
        private static IEnumerable<Type> GetGenericInterfaceTypes(Type typeImplements, Type typeGenericInterface)
        {
            return typeImplements
                .GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeGenericInterface)
                .SelectMany(i => i.GetGenericArguments())
                .ToArray();
        }

        #endregion
    }
}