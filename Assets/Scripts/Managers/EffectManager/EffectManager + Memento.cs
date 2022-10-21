using System;
using System.Collections.Generic;
using RollABall.Args;
using RollABall.Infrastructure.Memento;
using RollABall.ScriptableObjects;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    // Use Memento Pattern
    public partial class EffectManager : IMementoOrganizer<List<EffectSaveArgs>>
    {
        #region Links
    
        [field: Space (10)]
        [field: Header("Memento")]
        [field: SerializeField] private EffectManagerCaretaker Caretaker { get; set; }
    
        #endregion
    
        #region Properties
    
        public List<EffectSaveArgs> State { get; set; } = new ();
    
        #endregion
    
        #region Functionality
    
        public List<EffectSaveArgs> MakeState()
        {
            throw new System.NotImplementedException();
        }

        public IMemento<List<EffectSaveArgs>> Save()
        {
            return new Memento<List<EffectSaveArgs>>(State, "Effects");
        }

        public void Load(IMemento<List<EffectSaveArgs>> memento)
        {
            if (memento is not Memento<List<EffectSaveArgs>>)
            {
                throw new Exception("Unknown memento class " + memento.ToString());
            }
            
            State = memento.State;
            InitManager(true);
        }
    
        #endregion
    }
}