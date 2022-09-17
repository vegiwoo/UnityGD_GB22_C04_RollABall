using System;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity
{
    /// <summary>
    /// Base class for all objects that a character can interact with.
    /// </summary>
    public abstract class InteractiveObject : MonoBehaviour
    {
        #region Properties
        
        protected List<string> CompareTags { get; set; }
        
        #endregion
        
        #region MonoBehaviour methods

        protected virtual void Start()
        {
            CompareTags = new List<string>(4);
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            if(!CompareTags.Contains(collision.gameObject.tag)) return;
            Interaction(collision.gameObject.tag);
        }
        
        #endregion

        #region Functionality
        protected abstract void Interaction(string tag);
        
        #endregion
    }
}