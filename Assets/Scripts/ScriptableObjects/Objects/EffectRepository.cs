using System;
using System.Collections.Generic;
using System.Linq;
using RollABall.Infrastructure.Repository;
using RollABall.Interactivity.Bonuses;
using RollABall.Stats;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.ScriptableObjects 
{
    [CreateAssetMenu(fileName = "EffectRepository", menuName = "RollABall/Objects/EffectRepository", order = 1)]
    public class EffectRepository : ScriptableObject, IRepositoryFindable<Guid, IEffectable>, IRepositoryUpdatable<Guid, IEffectable>
    {
        public int Count => _effects.Count();
        
        private readonly Dictionary<Guid,IEffectable> _effects = new ();
        
        public void Init(EffectStats stats)
        {
            try
            {
                foreach (var statsEffect in stats.effects)
                {
                    var effectable = (IEffectable)statsEffect;
                    Insert(effectable.Id, effectable);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
        // Finding
        public void Insert(Guid key, IEffectable value)
        {
            _effects.Add(key, value);
        }
        
        public KeyValuePair<Guid, IEffectable> FindOnceByFilter(Func<IDictionary<Guid, IEffectable>, KeyValuePair<Guid, IEffectable>> func)
        {
            return func(_effects);
        }
        
        public KeyValuePair<Guid, IEffectable> FindOnceByFilter(Func<IEffectable, bool> isMatch)
        {
            throw new NotImplementedException();
        }
        
        // Updating
        public void UpdateAllWithAction(Action<KeyValuePair<Guid, IEffectable>> action)
        {
            throw new NotImplementedException();
        }

        public void UpdateOnceWithAction(Guid key, Action<KeyValuePair<Guid, IEffectable>> action)
        {
            throw new NotImplementedException();
        }
    }
}