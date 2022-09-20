
// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Bonuses
{
    public class AddHitPointsEffectFactory : EffectFactory
    {
        #region Constructors
        public AddHitPointsEffectFactory(float power, float duration) : base(power, duration) { }
        #endregion
        
        #region Functionality
        
        public override IEffectable GetEffect()
        {
            return new AddHitPointsEffect(Power, Duration);
        }
        
        #endregion
    }
}