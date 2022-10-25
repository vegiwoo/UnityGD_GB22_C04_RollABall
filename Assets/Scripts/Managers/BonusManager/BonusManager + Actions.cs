using System;
using System.Collections.Generic;
using System.Linq;
using RollABall.Args;
using RollABall.Interactivity.Bonuses;
using UnityEngine;
using static UnityEngine.Debug;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    // BonusManager + Actions
    public partial class BonusManager
    {
        #region Manager actions 
        
        protected override void NewGameAction()
        {
            SavedState = new List<ISavableArgs>(new List<BonusSaveArgs>());
            
            var counter = 0;
            while (counter < bonusPoints.Length)
            {
                var point = bonusPoints[counter];
                    
                var buffBonusItem = CreateBonusAndObject(EffectType.Buff, null, point);
                var debuffBonusItem = CreateBonusAndObject(EffectType.Debuff, null, point);
                    
                try
                {
                    BonusRepository.Insert(point,new [] { buffBonusItem, debuffBonusItem });
                }
                catch (Exception e)
                {
                    LogException(e);
                }

                ++counter;
            }
            
            // Activation of a random bonus from a pair at each point
            BonusRepository.UpdateAllWithAction(RandomActivateAndSubscribeAction);
            
            // Placement notify
            BonusManagerEvent.Notify(new BonusManagerArgs(bonusPoints,null,null,false));
        }

        protected override void SaveGameAction()
        {
            // MakeState(); ???
        }

        protected override void LoadGameAction(SaveGameArgs args)
        {
            StopAllCoroutines();

            if (args.BonusSaveArgs is not null)
            {
                SavedState = new List<ISavableArgs>(args.BonusSaveArgs);
            }
            
            // Clear repo
            BonusRepository.UpdateAllWithAction(UnsubscribeAndDestroyAction);
            BonusRepository.RemoveAll();
                
            // Cleanup notify
            BonusManagerEvent.Notify(new BonusManagerArgs(null,null,null,true));
                
            // Make items from loading file
            const float pointTolerance = 0.05f;
                
            foreach (var bonusPoint in bonusPoints)
            {
                var entities = SavedState
                    .Cast<BonusSaveArgs>()
                    .Where(it =>
                {
                    Vector3 position;
                    return Math.Abs(it.Point.PosX - (position = bonusPoint.position).x) < pointTolerance &&
                           Math.Abs(it.Point.PosZ - position.z) < pointTolerance;
                }).ToList();

                var pair = new KeyValuePair<Transform, BonusItem[]>(bonusPoint, new BonusItem[2]);

                for (var i = 0; i < entities.Count(); i++)
                {
                    var bonusItem = CreateBonusAndObject(null, entities[i].Effect, bonusPoint, entities[i].IsActive);
                    pair.Value[i] = bonusItem;
                }

                try
                {
                    BonusRepository.Insert(pair.Key, pair.Value);
                }
                catch (Exception e)
                {
                    LogException(e);
                }
            }
        }

        protected override void RestartGameAction()
        {
            StopAllCoroutines();
            SavedState = new List<ISavableArgs>(new List<BonusSaveArgs>());
            
            if (BonusRepository.Count > 0)
            {
                BonusRepository.UpdateAllWithAction(UnsubscribeAndUnactivateAction);
            }
            
            BonusRepository.UpdateAllWithAction(RandomActivateAndSubscribeAction);
        }

        private void MakeState()
        {
            var items =
                from bonusPair in BonusRepository.FindAllValues()
                from bonus in bonusPair
                select new BonusSaveArgs(
                    bonus.Bonus.Point.position, 
                    bonus.Bonus.Effect as Effect,
                    bonus.BonusGo.activeInHierarchy);
            
            SavedState = new List<ISavableArgs>(items.ToList());
        }
        
        #endregion
        
        #region Repository actions 
        
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
        
        #endregion
    }
}