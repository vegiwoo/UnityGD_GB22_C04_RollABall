using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace RollABall.Test
{
    public interface IRepositoryFindable<K,V>
    {
        KeyValuePair<K,V> FindOnceByFilter(Func<V, bool> isMatch);
    }
}