
// ReSharper disable once CheckNamespace

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameDevLib.Enums;
using RollABall.Args;
using RollABall.Interactivity.Bonuses;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    // BonusManager + Bonus control
    public partial class BonusManager
    {
        /// <summary>
        /// Creates a new bonus and a game object for it.
        /// </summary>
        /// <param name="effectType">Type of effect for bonus.</param>
        /// <param name="point">Bonus point.</param>
        /// <param name="isActive">Whether the item being created is active.</param>
        /// <param name="effect">Ready effect to assign to the bonus (optional).</param>
        /// <returns></returns>
        private BonusItem CreateBonusAndObject(EffectType? effectType, Effect? effect, Transform point, bool isActive = false)
        {
            IEffectable createdEffect;
            if (effect == null && effectType.HasValue)
            {
                // Create effect 
                KeyValuePair<Guid, IEffectable> RandomEffectByType(IDictionary<Guid, IEffectable> collection)
                {
                    var effectsByType = collection
                        .Where(el => el.Value.Type == effectType)
                        .ToList();
                
                    var randomIndex = systemRandom.Next(0, effectsByType.Count);
                
                    return effectsByType[randomIndex];
                }
            
                createdEffect = EffectRepository.FindOnceByFilter(RandomEffectByType).Value;
            }
            else
            {
                createdEffect = effect;
            }

            // Create game object
            var o = Instantiate(bonusPrefab, point.position, point.rotation);
            o.tag = GameData.BonusTag;
            o.SetActive(isActive);
            
            // Add and init bonus 
            IBonusable bonus = o.AddComponent<Bonus>();
            bonus.Init(createdEffect, point);

            if (isActive)
            {
                bonus.InteractiveNotify += OnBonusNotify;
            }

            // Set point as parent for new object
            o.transform.parent = point;
            
            return new BonusItem(bonus, o);
        }
        
        /// <summary>
        /// Called when a bonus collision occurs. 
        /// </summary>
        /// <param name="bonus">Bonus that came across.</param>
        /// <param name="tagElement">Tag of element that the bonus encountered.</param>
        private void OnBonusNotify(IBonusable bonus, string tagElement)
        {
            if (string.Equals(tagElement, GameData.PlayerTag))
            {
                ApplyEffectEvent.Notify(bonus.Effect);
            }
            
            _audioIsPlaying.PlaySound(bonus.Effect.Type == EffectType.Buff ? 
                SoundType.Positive : 
                SoundType.Negative);
            
            StartCoroutine(ToggleBonusCoroutine(bonus));
        }
        
        private IEnumerator ToggleBonusCoroutine(IBonusable bonusable)
        {
            bool IsMatch(IEnumerable<BonusItem> items)
            {
                return items.Any(el => el.Bonus == bonusable);
            }

            var existingPair = BonusRepository.FindOnceByFilter(IsMatch);
            
            // Unsubscribe and deactivation bonus
            BonusRepository.UpdateOnceWithAction(existingPair.Key, UnsubscribeAndUnactivateAction);

            // Deactivation bonus point notify
            BonusManagerEvent.Notify(new BonusManagerArgs(null,existingPair.Key,null,false));

            // Audio Completion Waiting
            var isSoundPlayed = false;
            _audioIsPlaying.AudioTriggerNotify += (played) => { isSoundPlayed = played; };
            
            // Delay 
            yield return new WaitUntil(() => isSoundPlayed); 
            yield return new WaitForSeconds(stats.DelayAfterRemove);
            
            // Toggle bonus in pair
            void ToggleAction(KeyValuePair<Transform, BonusItem[]> pair)
            {
                if (pair.Value.Length != 2) return;

                var otherBonus = existingPair.Value.First(el => el.Bonus != bonusable);
                otherBonus.Bonus.InteractiveNotify += OnBonusNotify;
                otherBonus.BonusGo.SetActive(true);
            }

            BonusRepository.UpdateOnceWithAction(existingPair.Key, ToggleAction);
            
            // Activation point notify
            BonusManagerEvent.Notify(new BonusManagerArgs(null,null,existingPair.Key,false));
        }
    }
}

