using System.Collections.Generic;
using RollABall.Interactivity.Bonuses;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    public partial class BonusManager
    {
        /// <summary>
        /// Unsubscribes from events and unactivate of all objects on point.
        /// </summary>
        /// <param name="value">Value to update.</param>
        private void UnsubscribeAndUnactivateAction(KeyValuePair<Transform, BonusItem[]> value)
        {
            // Unsubscribing from collision events with a bonus in pair
            foreach (var item in value.Value)
            {
                item.Bonus.InteractiveNotify -= OnBonusNotify;
            }

            // Deactivation of all game objects in pair
            var bonusGameObjectsInPointCount = value.Key.childCount;
            for (var i = 0; i < bonusGameObjectsInPointCount; ++i)
            {
                var bonusGameObject = value.Key.GetChild(i);
                bonusGameObject.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Unsubscribes from events and destroy all objects on point.
        /// </summary>
        /// <param name="value">Value to update.</param>
        private void UnsubscribeAndDestroyAction(KeyValuePair<Transform, BonusItem[]> value)
        {
            // Unsubscribing from collision events with a bonus in pair
            foreach (var item in value.Value)
            {
                item.Bonus.InteractiveNotify -= OnBonusNotify;
            }
            
            // Remove of all game objects in pair
            var bonusGameObjectsInPointCount = value.Key.childCount;
            for (var i = 0; i < bonusGameObjectsInPointCount; ++i)
            {
                var bonusGoTransform = value.Key.GetChild(i);
                if (bonusGoTransform != null && bonusGoTransform.gameObject != null)
                {
                    Destroy(bonusGoTransform.gameObject);
                }
            }
        }
        
        /// <summary>
        /// Randomly activates one of bonuses on point.
        /// </summary>
        /// <param name="value">Value to update.</param>
        private void RandomActivateAndSubscribeAction(KeyValuePair<Transform, BonusItem[]> value)
        {
            var element = value.Value[systemRandom.Next(0, value.Value.Length)];
            
            element.BonusGo.SetActive(true);
            element.Bonus.InteractiveNotify += OnBonusNotify;
        }
    }
}

