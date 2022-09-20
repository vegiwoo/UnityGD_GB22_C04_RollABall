
// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Bonuses
{
    public class SpeedUpEffectFactory : EffectFactory
    {
        #region Constructors
        
        public SpeedUpEffectFactory(float power, float duration) : base(power, duration) { }
        
        #endregion
        
        #region Functionality
        
        public override IEffectable GetEffect()
        {
            return new SpeedUpEffect(Power, Duration);
        }
        
        #endregion
    }
}