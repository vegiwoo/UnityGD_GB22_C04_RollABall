using System;
using System.Collections.Generic;
using System.Linq;
using RollABall.Infrastructure.Repository;
using RollABall.Interactivity.Bonuses;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.ScriptableObjects
{
    [CreateAssetMenu(fileName = "BonusRepository", menuName = "RollABall/Objects/BonusRepository", order = 0)]
    public class BonusRepository : ScriptableObject, IRepositoryUpdatable<Transform,BonusItem[]>, IRepositoryFindable<Transform,BonusItem[]>
    {
        // TODO: Переписать на SortedDictionary<>, для этого BonusItem[] обернуть в класс и реализовать IComparable<>
        private readonly Dictionary<Transform, BonusItem[]> _storage = new ();
        
        public int Count => _storage.Count;

        // If there is a duplicate key, an ArgumentException is thrown (call ONLY in a try/catch/finally block)
        public void Insert(Transform key, BonusItem[] value)
        {
            _storage.Add(key, value);
        }

        public void UpdateAllWithAction(Action<KeyValuePair<Transform, BonusItem []>> action)
        {
            foreach (var items in _storage)
            {
                UpdateOnceWithAction(items.Key, action);
            }
        }
        
        public void UpdateOnceWithAction(Transform key, Action<KeyValuePair<Transform, BonusItem []>> action)
        {
            if (!_storage.ContainsKey(key)) return;
            
            var value = _storage[key];
            action(new KeyValuePair<Transform, BonusItem[]>(key, value));
        }

        public KeyValuePair<Transform,  BonusItem[]> FindOnceByFilter(Func<BonusItem[], bool> isMatch)
        {
            var relevantItems = 
                from el in _storage
                where isMatch(el.Value)
                select el;

            return relevantItems.First();
        }

        public KeyValuePair<Transform, BonusItem[]> FindOnceByFilter(Func<IDictionary<Transform, BonusItem[]>, 
            KeyValuePair<Transform, BonusItem[]>> isMatch)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<BonusItem[]> FindAll()
        {
            return _storage
                .Select(el => el.Value)
                .ToList();
        }
    }
}