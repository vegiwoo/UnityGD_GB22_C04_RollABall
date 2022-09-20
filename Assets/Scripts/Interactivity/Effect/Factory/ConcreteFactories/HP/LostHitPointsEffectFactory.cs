
// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Bonuses
{
    public class LostHitPointsEffectFactory : EffectFactory
    {
        #region Constructors
        public LostHitPointsEffectFactory(float power, float duration) : base(power, duration) { }
        
        #endregion
        
        #region Functionality
        
        public override IEffectable GetEffect()
        {
            return new LostHitPointsEffect(Power, Duration);
        }
        
        #endregion
    }
}