using System;
using System.Linq;
using System.Collections.Generic;
using RollABall.Infrastructure.Repository;
using UnityEngine;
using UnityEngine.UI;

// ReSharper disable once CheckNamespace
namespace RollABall.ScriptableObjects
{
    [CreateAssetMenu(fileName = "RadarObjectsRepository", menuName = "RollABall/Objects/RadarObjectsRepository", order = 2)]
    public class RadarObjectsRepository : ScriptableObject, IRepositoryFindable<Transform, RawImage>, 
        IRepositoryUpdatable<Transform, RawImage>, IRepositoryRemovable
    {
        private readonly Dictionary<Transform,RawImage> _points = new ();
        
        public int Count => _points.Count();

        public KeyValuePair<Transform, RawImage> FindOnceByFilter(Func<RawImage, bool> isMatch)
        {
            throw new NotImplementedException();
        }

        public KeyValuePair<Transform, RawImage> FindOnceByFilter(Func<IDictionary<Transform, RawImage>, KeyValuePair<Transform, RawImage>> func)
        {
            throw new NotImplementedException();
        }

        public IDictionary<Transform, RawImage> FindAll()
        {
            return new Dictionary<Transform, RawImage>(_points);
        }

        public IEnumerable<RawImage> FindAllValues()
        {
            return _points
                .Select(el => el.Value)
                .ToList();
        }

        public void Insert(Transform key, RawImage value)
        {
            _points.Add(key, value);
        }

        public void UpdateAllWithAction(Action<KeyValuePair<Transform, RawImage>> action)
        {
            foreach (var items in _points)
            {
                UpdateOnceWithAction(items.Key, action);
            }
        }

        public void UpdateOnceWithAction(Transform key, Action<KeyValuePair<Transform, RawImage>> action)
        {
            if (!_points.ContainsKey(key)) return;
            
            var value = _points[key];
            action(new KeyValuePair<Transform, RawImage>(key, value));
        }
        
        public void RemoveAll()
        {
            _points.Clear();
        }
    }
}

