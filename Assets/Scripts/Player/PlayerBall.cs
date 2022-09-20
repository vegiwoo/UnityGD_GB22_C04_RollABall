
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
    }
}