
// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Bonuses
{
    public class SlowDownEffectFactory : EffectFactory
    {
        #region Constructors
        
        public SlowDownEffectFactory(float power, float duration) : base(power, duration) { }
        
        #endregion
        
        #region Functionality
        
        public override IEffectable GetEffect()
        {
            return new SlowDownEffect(Power, Duration);
        }
        
        #endregion
    }
}