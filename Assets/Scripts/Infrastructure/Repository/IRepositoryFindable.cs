using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace RollABall.Infrastructure.Repository
{
    public interface IRepositoryFindable<K,V>
    {
        KeyValuePair<K,V> FindOnceByFilter(Func<V, bool> isMatch);
        KeyValuePair<K,V> FindOnceByFilter(Func<IDictionary<K,V>,  KeyValuePair<K,V>> func);
    }
}