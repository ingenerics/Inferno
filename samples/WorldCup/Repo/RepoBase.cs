using System;
using System.Collections.Generic;
using System.Linq;

namespace WorldCup.Repo
{
    public class RepoBase<TKey, TValue> : IRepo<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _dictionary;

        public RepoBase(TValue[] items, Func<TValue, TKey> keySelector)
        {
            _dictionary = items.ToDictionary(keySelector);
        }

        public TValue Get(TKey key) => _dictionary.GetValueOrDefault(key);

        public List<TValue> GetAll() => _dictionary.Values.ToList();
    }
}
