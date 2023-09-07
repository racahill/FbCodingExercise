using System.Configuration;
using System.Collections.Concurrent;

namespace Finbourne.Caching
{
    public sealed class ObjectCache : IObjectCache
    {
        private const string cacheLimitConfigKey = "objectCacheLimit";

        private static int cacheLimit;

        private ObjectCache()
        {
            // Retrieve the configured cache limit 
            cacheLimit = GetCacheLimit(cacheLimitConfigKey);
            // Log the creation of the class object
            Console.WriteLine("ObjectCache Instance Created");
        }

        // Simple and thread safe implementation of a singleton pattern.
        private static readonly Lazy<ObjectCache> lazy = new(() => new ObjectCache());

        // Public property to access instance. Singe point of entry.
        public static ObjectCache Instance 
        { 
            get { return lazy.Value; } 
        }

        // Represents a thread-safe collection of key/value pairs that can be accessed by multiple threads concurrently. Will contain an arbitary set of objects.
        private ConcurrentDictionary<object, CacheItem> concurrentDictionary = new ConcurrentDictionary<object, CacheItem>();
        
        //This variable is going to store the Singleton Instance
        //Initializing the Variable at the time of class start-up 
        //and make it ready to be used in the Future
        /* private static readonly ObjectCache singletonInstance = new ObjectCache(); */
        
        //The following Static Method is going to return the Singleton Instance
        //This is Thread-Safe as it uses Eager Loading
        /* public static ObjectCache GetInstance()
        {
            return singletonInstance;
        }*/

        //Constructor needs to be Private in order to restrict 
        //class instantiation from Outside of this class
        /*private ObjectCache()
        {
            Console.WriteLine("SingletonCache Instance Created");
        }*/
        
        //The following methods can be accessed from outside of the class by using the Singleton Instance

        //This method is used to add a Key-Value Pair into the Cache
        public bool AddOrUpdate(object key, CacheItem value)
        {
            //Check cache limit and remove/evict item with oldest LastUpdated value of CacheItem
            if (concurrentDictionary.Count() < cacheLimit)
            {
                return concurrentDictionary.TryAdd(key, value);
            }
            else
            {
                return RemoveEvicteeAndAdd(key, value);
            }
        }

        // Return CacheItem value for given key if key present else null
        public CacheItem? Get(object key)
        {
            CacheItem? cachedItem;
            if (concurrentDictionary.ContainsKey(key))
            {
                concurrentDictionary.TryGetValue(key, out cachedItem);
                return cachedItem;
            }
            return null;
        }

        // Removes given key from the cache. Sends notification in case of eviction and returns a null CacheItem if false.
        public CacheItem? Remove(object key)
        {
            CacheItem? value;

            if (key != null)
            {
                concurrentDictionary.TryRemove(key, out value);
                return value;
            }
 
            return null;
        }

        // Removes all entries from the cache
        public void Clear()
        {
            concurrentDictionary.Clear();
        }

        // Send notification to user. 
        private string SendNotification(object key, CacheItem? value, string message)
        {
            //TODO: Construct message with key, object type and message and present via in code notification object. For now on console.
            Console.WriteLine("Notification Sent: " + message );

            return message;
        }

        // Method to retreive cache limit for memory preservation
        private int GetCacheLimit(string keyToCheck)
        {
            int cacheLimit;

            if (Array.Exists(ConfigurationManager.AppSettings.AllKeys, key => key == keyToCheck))
            {
                int.TryParse(ConfigurationManager.AppSettings[keyToCheck], out cacheLimit);
                Console.WriteLine($"Key '{keyToCheck}' exists with value '{cacheLimit}'.");
                return cacheLimit;
            }
            else
            {
                Console.WriteLine($"Key '{keyToCheck}' does not exist in appSettings.");
                return 0;
            }
        }

        // Remove evictee based on LastUpdatedDate and add new entry
        private bool RemoveEvicteeAndAdd(object key, CacheItem value)
        {
            // Find evictee. Order the dictionary by LastUpdated CacheItem attribute and get oldest
            KeyValuePair<object, CacheItem> removalItem = concurrentDictionary.OrderBy(v => v.Value.LastUpdated).FirstOrDefault();

            // Remove oldest
            if (concurrentDictionary.TryRemove(removalItem))
            {
                SendNotification(key, value, "Object Cache Eviction");
            }

            // Add new
            if (key != null)
            {
                return concurrentDictionary.TryAdd(key, value);
            }
            
            return false;
        }
    }
}