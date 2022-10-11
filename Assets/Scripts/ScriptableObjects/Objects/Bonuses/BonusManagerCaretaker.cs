#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using RollABall.Infrastructure.Memento;
using RollABall.Interactivity.Bonuses;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.ScriptableObjects
{
    [CreateAssetMenu(fileName = "BonusManagerCaretaker", menuName = "RollABall/Caretakers/BonusManagerCaretaker",
        order = 0)]
    public class BonusManagerCaretaker : Caretaker<List<BonusManagerStateItem>>
    {
        private readonly JsonSerializerSettings _jsonFormatSetting = new ()
        {
            Formatting = Formatting.Indented,
            DateFormatHandling = DateFormatHandling.IsoDateFormat
        };

        public override void Save()
        {
            // Get memento
            var memento = Originator.Save();
            
            // Check Directory
            var path = CheckDirectory(SavedPath, true);
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
            savedMemento = (memento, savedPath);
                
            Debug.Log($"Bonuses Saved ({memento.Name})");
        }

        protected override void Preload()
        {
            var dir = CheckDirectory(SavedPath, false);
            if (dir is not null)
            {
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
                    var memento = JsonConvert.DeserializeObject<BonusManagerMemento>(jsonString, _jsonFormatSetting);

                    // Saved in savedMemento
                    if (memento is not null)
                    {
                        savedMemento = (memento, oneFilePath);
                        Debug.Log("Game save preloaded.");
                    }
                    else
                    {
                        Debug.Log("No saved games.");
                    }
                }
                else
                {
                    Debug.Log("No saved games.");
                }
            }
        }

        public override void Load()
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

        private static string? CheckDirectory(string savedPath, bool create)
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
    }
}