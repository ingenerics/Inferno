using System.Collections.Generic;

namespace WorldCup.Repo
{
    public interface IRepo<in TKey, TValue>
    {
        TValue Get(TKey key);
        List<TValue> GetAll();
    }
}
