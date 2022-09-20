
// ReSharper disable once CheckNamespace
namespace RollABall.Interactivity.Bonuses
{
    public class AddGamePointsEffectFactory : EffectFactory
    {
        #region Constructors
        
        public AddGamePointsEffectFactory(float power, float duration) : base(power, duration) { }
        
        #endregion
        
        #region Functionality
        
        public override IEffectable GetEffect()
        {
            return new AddGamePointsEffect(Power, Duration);
        }
        
        #endregion
    }
}