using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace RollABall.Test
{
    public interface IRepositoryUpdatable<K,V>
    {
        /// <summary>
        /// Current number of items in repository.
        /// </summary>
        int Count { get; }
        
        /// <summary>
        /// Inserts a new element into collection.
        /// </summary>
        /// <param name="key">Inserting key.</param>
        /// <param name="value">Inserting value.</param>
        void Insert(K key, V value);

        /// <summary>
        /// Updates all elements by executing an Action.
        /// </summary>
        /// <param name="action">Action to execute on element</param>
        void UpdateAllWithAction(Action<KeyValuePair<K,V>> action);
        public void UpdateOnceWithAction(K key, Action<KeyValuePair<K, V>> action);
    }
}