using System.Collections.Generic;
using System.IO;
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
        public override void Init(IMementoOrganizer<List<BonusManagerStateItem>> originator, string rootPath)
        {
            base.Init(originator, rootPath);
            
            // Достать все предыдущие сохрания, обновить mementos !!! 
        }
        
        public override void Save()
        {
            var memento = Originator.Save();
            
            // Check Directory
            if (!Directory.Exists(SavedPath))
            {
                Directory.CreateDirectory(SavedPath);
            }
            
            var savedPath = Path.Combine(SavedPath, $"{memento.Name}.json");
            //Debug.Log(savedPath);

            using var writer = new StreamWriter(savedPath, false, Encoding.Default);
            
            var jsonFormatSetting = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                DateFormatHandling = DateFormatHandling.IsoDateFormat
            };
            var json = JsonConvert.SerializeObject(memento, jsonFormatSetting);
            writer.Write(json);
            Debug.Log("Bonuses Saved.");
            
            //mementos.Add(Originator.Save());
        }

        public override void Load()
        {
            string[] files = Directory.GetFiles(SavedPath);
            foreach (var file in files)
            {
                Debug.Log(file);
            }


            //{
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
            // }
        }
    }
}