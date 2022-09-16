using System;
using RollABall.Interactivity.Bonuses;
using RollABall.Interactivity.Effects;

// ReSharper disable once CheckNamespace
namespace RollABall.Player
{
    public class PlayerBall : Player
    {
        #region MonoBehavior methods

        protected override void Start()
        {
            base.Start();
            transform.gameObject.tag = GameData.PlayerTag;
        }

        private void FixedUpdate()
        {
            Move();
        }

        #endregion
        
        #region Functionality

        private void ApplyEffect()
        {
            
        }
        
        #endregion
    }


}