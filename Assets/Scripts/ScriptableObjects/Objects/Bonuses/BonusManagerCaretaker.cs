#nullable enable

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using RollABall.Infrastructure.Memento;
using RollABall.Interactivity.Bonuses;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.ScriptableObjects
{
    [CreateAssetMenu(fileName = "BonusManagerCaretaker", menuName = "RollABall/Caretakers/BonusManagerCaretaker",
        order = 0)]
    public class BonusManagerCaretaker : Caretaker<List<BonusManagerStateItem>,BonusManagerMemento>
    {
        protected override void Preload()
        {
            var dir = CheckExistenceDirectory(SavedPath, false);
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
                    var memento = JsonConvert.DeserializeObject<BonusManagerMemento>(jsonString, jsonFormatSetting);

                    // Saved in savedMemento
                    if (memento is not null)
                    {
                        savedMemento = (memento, oneFilePath);
                        Debug.Log("Game save (bonuses) preloaded.");
                    }
                    else
                    {
                        Debug.Log("No saved  (bonuses) games.");
                    }
                }
                else
                {
                    Debug.Log("No saved  (bonuses) games.");
                }
            }
        }
    }
}