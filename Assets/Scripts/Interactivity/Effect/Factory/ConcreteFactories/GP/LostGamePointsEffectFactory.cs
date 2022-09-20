
// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Bonuses
{
    public class LostGamePointsEffectFactory : EffectFactory
    {
        #region Constructors
        public LostGamePointsEffectFactory(float power, float duration) : base(power, duration) { }
        #endregion
        
        #region Functionality
        
        public override IEffectable GetEffect()
        {
            return new LostGamePointsEffect(Power, Duration);
        }
        
        #endregion
    }
}