using RollABall.Interactivity.Bonuses;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity
{
    /// <summary>
    /// Base class for all objects that a character can interact with.
    /// </summary>
    public abstract class InteractiveObject : MonoBehaviour
    {
        #region MonoBehaviour methods

        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.gameObject.CompareTag(GameData.PlayerTag)) return;
            Interaction();
        }
        
        #endregion

        protected abstract void Interaction();
    }
}