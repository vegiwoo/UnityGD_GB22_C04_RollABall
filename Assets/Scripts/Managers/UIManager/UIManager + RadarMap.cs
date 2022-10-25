using System.Linq;
using RollABall.Events;
using RollABall.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

// ReSharper disable once CheckNamespace
namespace RollABall.Managers
{
    // UIManager + RadarMap
    // Reference: https://timcoster.com/2020/03/25/unity-enemy-radar-tutorial/ 
    public partial class UIManager
    {
        #region Links 
        [field:Header("Radar map")]
        [field: SerializeField] private RawImage RadarMapImage { get; set; }
        [field: SerializeField] private RawImage BonusPointImage { get; set; }
        [field: SerializeField] private BonusManagerEvent BonusManagerEvent { get; set; }
        [field: SerializeField] private  RadarObjectsRepository RadarObjectsRepository { get; set; }
        [field: SerializeField] private float RadarDistance { get; set; } = 15;
        [field: SerializeField] private float BlipSize { get; set; }= 10;
        [field: SerializeField] public Transform Player { get; set; }
        
        #endregion
        
        #region Fields
        
        public bool usePlayerDirection = false;
        private float _radarWidth, _radarHeight, _blipWidth, _blipHeight;
        
        #endregion
        
        #region Functionality
        
        private void DisplayBonusesOnMap()
        {
            var playerPos = Player.position;

            foreach (var target in RadarObjectsRepository
                         .FindAll()
                         .Where(i => i.Value.gameObject.activeInHierarchy))
            {
                var targetPos = target.Key.position;
                var distanceToTarget = Vector3.Distance(targetPos, playerPos);

                if (!(distanceToTarget <= RadarDistance)) continue;
                
                var normalisedTargetPosition = NormalisedPosition(playerPos, targetPos);
                var bonusPointPosition = CalculateBonusPointPosition(normalisedTargetPosition);

                var rt = target.Value.GetComponent<RectTransform>();
                rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left,bonusPointPosition.x, _blipWidth);
                rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top,bonusPointPosition.y, _blipHeight);
            }
        }
    
        private Vector3 NormalisedPosition(Vector3 playerPos, Vector3 targetPos) {
            var normalisedTargetX = (targetPos.x - playerPos.x)/RadarDistance;
            var normalisedTargetZ = (targetPos.z - playerPos.z)/RadarDistance;
            
            return new Vector3(normalisedTargetX, 0, normalisedTargetZ);
        }
        
        private Vector2 CalculateBonusPointPosition(Vector3 targetPos) {
            // find the angle from the player to the target.
            var angleToTarget = Mathf.Atan2(targetPos.x,targetPos.z) * Mathf.Rad2Deg;
 
            // The direction the player is facing.
            var anglePlayer = usePlayerDirection? Player.eulerAngles.y : 0;
 
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
        
        #endregion
    }
}