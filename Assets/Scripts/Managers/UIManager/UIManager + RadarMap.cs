using System;
using System.Collections.Generic;
using System.Linq;
using GameDevLib.Interfaces;
using RollABall.Args;
using RollABall.Events;
using RollABall.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Debug;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    public partial class UIManager : GameDevLib.Interfaces.IObserver<BonusManagerArgs>
    {
        #region Links 
        
        [field:Header("Radar map")]
        [field: SerializeField] private RawImage RadarMapImage { get; set; }
        [field: SerializeField] private RawImage BonusPointImage { get; set; }
        [field: SerializeField] private BonusManagerEvent BonusManagerEvent { get; set; }
        [field: SerializeField] private  RadarObjectsRepository RadarObjectsRepository { get; set; }
        
        #endregion
        
        #region Properties 
        
        // Radar map
        private Vector3 PlayerPosition { get; set; }
        
        #endregion
        
        #region Fields
        
        private float _radarWidth, _radarHeight, _blipWidth, _blipHeight;
        
        #endregion
        
        #region Functionality
        
        private void DisplayBonusesOnMap()
        {
            PlayerPosition = player.position;

            foreach (var target in RadarObjectsRepository
                         .FindAll()
                         .Where(i => i.Value.gameObject.activeInHierarchy))
            {
                var targetPos = target.Key.position;
                var distanceToTarget = Vector3.Distance(targetPos, PlayerPosition);

                if (!(distanceToTarget <= radarDistance)) continue;
                
                var normalisedTargetPosition = NormalisedPosition(PlayerPosition, targetPos);
                var bonusPointPosition = CalculateBonusPointPosition(normalisedTargetPosition);

                var rt = target.Value.GetComponent<RectTransform>();
                rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left,bonusPointPosition.x, _blipWidth);
                rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top,bonusPointPosition.y, _blipHeight);
            }
        }
    
        private Vector3 NormalisedPosition(Vector3 playerPos, Vector3 targetPos) {
            var normalisedTargetX = (targetPos.x - playerPos.x)/radarDistance;
            var normalisedTargetZ = (targetPos.z - playerPos.z)/radarDistance;
            
            return new Vector3(normalisedTargetX, 0, normalisedTargetZ);
        }
        
        private Vector2 CalculateBonusPointPosition(Vector3 targetPos) {
            // find the angle from the player to the target.
            var angleToTarget = Mathf.Atan2(targetPos.x,targetPos.z) * Mathf.Rad2Deg;
 
            // The direction the player is facing.
            var anglePlayer = usePlayerDirection? player.eulerAngles.y : 0;
 
            // Subtract the player angle, to get the relative angle to the object. Subtract 90
            // so 0 degrees (the same direction as the player) is Up.
            var angleRadarDegrees = angleToTarget - anglePlayer - 90;
 
            // Calculate the xy position given the angle and the distance.
            var normalisedDistanceToTarget = targetPos.magnitude;
            var angleRadians = angleRadarDegrees * Mathf.Deg2Rad;
            var blipX = normalisedDistanceToTarget * Mathf.Cos(angleRadians);
            var blipY = normalisedDistanceToTarget * Mathf.Sin(angleRadians);
 
            // Scale the blip position according to the radar size.
            blipX *= _radarWidth * .5f;
            blipY *= _radarHeight * .5f;
 
            // Offset the blip position relative to the radar center
            blipX += (_radarWidth * .5f) - _blipWidth * .5f;
            blipY += (_radarHeight * .5f) - _blipHeight * .5f;
 
            return new Vector2(blipX, blipY);
        }
        
        public void OnEventRaised(ISubject<BonusManagerArgs> subject, BonusManagerArgs args)
        {
            if (args.ActivatePoints is not null)
            {
                RadarObjectsRepository.UpdateAllWithAction(UnactivateAndDestroyRadarPointAction);
                RadarObjectsRepository.RemoveAll();

                foreach (var point in args.ActivatePoints)
                {
                    var newItem = Instantiate(BonusPointImage);

                    try
                    {
                        newItem.transform.SetParent(RadarMapImage.transform);
                        RadarObjectsRepository.Insert(point, newItem);
                    }
                    catch (Exception e)
                    {
                        LogException(e);
                    }
                }
            }
            
            if (args.ActivatePoint is not null)
            {
                RadarObjectsRepository.UpdateOnceWithAction(args.ActivatePoint, ActivateRadarPointAction);
            }
            
            if (args.UnactivatePoint is not null)
            {
                RadarObjectsRepository.UpdateOnceWithAction(args.UnactivatePoint, UnactivateRadarPointAction);
            }
            
            if (args.RemoveAll)
            {
                RadarObjectsRepository.UpdateAllWithAction(UnactivateAndDestroyRadarPointAction);
                RadarObjectsRepository.RemoveAll();
            }
        }
        
        // Repository actions
        
        private static void ActivateRadarPointAction(KeyValuePair<Transform, RawImage> item)
        {
            var go = item.Value.gameObject;
            if (!go.activeInHierarchy)
            {
                item.Value.gameObject.SetActive(true);
            }
        }
        
        private static void UnactivateRadarPointAction(KeyValuePair<Transform, RawImage> item)
        {
            var go = item.Value.gameObject;
            if (go.activeInHierarchy)
            {
                item.Value.gameObject.SetActive(false);
            }
        }
        
        private static void UnactivateAndDestroyRadarPointAction(KeyValuePair<Transform, RawImage> item)
        {
            var go = item.Value.gameObject;
            go.SetActive(false); 
            Destroy(go);
        }
        
        #endregion
    }
}

