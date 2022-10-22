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

        private List<(IMemento<TState> memento, string path)> _mementos;
        
        private readonly JsonSerializerSettings _jsonFormatSetting = new ()
        {
            Formatting = Formatting.Indented,
            DateFormatHandling = DateFormatHandling.IsoDateFormat
        };
        
        #endregion
    
        #region Properties

        private IMementoOrganizer<TState> Originator { get; set;}
        private string SavedPath { get; set; }
        private string FilenamePrefix { get; set; }
        private static string SavedDir => "Saved";
        private string SavedEntityName { get; set; }
        
        private SaveFormat SaveFormat { get; set; }
        
        /// <summary>
        /// Number of saves that the caretaker must provide.
        /// </summary>
        private int NumberSaves { get; set; }
        
        #endregion

        #region Functionality
        public void Init(IMementoOrganizer<TState> originator, string dirName, string filenamePrefix,  
            int numberSaves = 3, SaveFormat saveFormat = SaveFormat.Json)
        {
            Originator = originator;
            SavedPath = Path.Combine(Application.persistentDataPath, $"{SavedDir}/{dirName}/");
            FilenamePrefix = filenamePrefix;
            SavedEntityName = dirName;

            SaveFormat = saveFormat;
            NumberSaves = numberSaves;
            
            _mementos = new List<(IMemento<TState> memento, string path)>();
            
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
            Debug.Log(savedPath);
            
            // Writing 
            using var writer = new StreamWriter(savedPath, false, Encoding.Default);

            switch (SaveFormat)
            {
                case SaveFormat.Json:
                    
                    var json = JsonConvert.SerializeObject(memento, _jsonFormatSetting);
                    writer.Write(json);
                    _mementos.Add((memento, savedPath));
                    
                    break;
            }

            Debug.Log($"{SavedEntityName} saved ({memento.Name})");
        }

        private void Preload()
        {
            Action<string> debugLogMessage = Debug.Log;

            var filePaths = GetFilesFromDirectory(CheckExistenceDirectory);
            if (filePaths.noConditionFilePaths.Any())
            {
                foreach (var noConditionFilePath in filePaths.noConditionFilePaths)
                {
                    File.Delete(noConditionFilePath); 
                }
            }
            
            if (filePaths.conditionFilePaths.Any())
            {
                foreach (var conditionFilePath in filePaths.conditionFilePaths)
                {
                    switch (SaveFormat)
                    {
                        case SaveFormat.Json:

                            using (var reader = new StreamReader(conditionFilePath))
                            {
                                var jsonString = reader.ReadToEnd();
                                var memento = JsonConvert.DeserializeObject<Memento<TState>>(jsonString, _jsonFormatSetting);

                                if (memento != null)
                                {
                                    _mementos.Add((memento,conditionFilePath));
                                }
                            }
                            
                            break;
                    }
                    
                    debugLogMessage($"Game save ({SavedEntityName}) preloaded.");
                }
            }
        }
        
        public void Load()
        {
            if(_mementos.Count > 0)
            {
                var firstMemento = _mementos.First().memento;
                Originator.Load(firstMemento);
                Debug.Log($"{SavedEntityName} Loaded ({firstMemento.Name})");
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
        /// Gets files from save directory and sorts them by condition.
        /// </summary>
        /// <param name="checkDirAction">Action to check directory.</param>
        /// <returns>A tuple containing conditional (last saves by number of NumberSaves) and non-conditional
        /// (all other) file paths.
        /// </returns>
        private (IEnumerable<string>? conditionFilePaths, IEnumerable<string>? noConditionFilePaths) 
            GetFilesFromDirectory(Func<string, bool, string?> checkDirAction)
        {
            var dir = checkDirAction.Invoke(SavedPath, false);
            if (dir == null)
            {
                Debug.Log("No saves.");
                return (null, null);
            }

            var allFilePathsInDir = Directory.GetFiles(dir);
            
            var conditionFilePaths = allFilePathsInDir
                .Where(p => p.Contains(FilenamePrefix))
                .OrderBy(File.GetCreationTimeUtc)
                .Take(NumberSaves)
                .ToList();

            var noConditionFilePaths = allFilePathsInDir
                .Where(fp => !conditionFilePaths.Contains(fp))
                .ToList();
            
            return (conditionFilePaths, noConditionFilePaths);
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