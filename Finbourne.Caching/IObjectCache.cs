public interface IObjectCache
{
    bool AddOrUpdate(object key, CacheItem value);
    CacheItem? Get(object key);
    CacheItem? Remove(object key);
    void Clear();
}