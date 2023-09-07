using Finbourne.Caching;

class Program
{
    static void Main(string[] args)
    {
        // Get the singleton instance
        ObjectCache instanceTest1 = ObjectCache.Instance;
        ObjectCache instanceTest2 = ObjectCache.Instance;

        // Check if both references point to the same instance to check object
        if (instanceTest1 == instanceTest2)
        {
            Console.WriteLine("Both references point to the same Singleton instance.");
        }
        else
        {
            Console.WriteLine("Singleton pattern is not working correctly.");
        }

        // Call a method to add an object to the cache
        bool test1 = instanceTest1.AddOrUpdate("Test1", new CacheItem("Test", DateTime.Now));

        // Call a method to get same object from the cache
        CacheItem? test2 = instanceTest1.Get("Test1");

        // Call a method to remove an item from the cache. No eviction.
        CacheItem? test3 = instanceTest1.Remove("Test1");

        // Populate to limit and test eviction
        for ( int i = 1; i < 5; i++)
        {
            bool result = instanceTest1.AddOrUpdate("Test"+i, new CacheItem("Test"+i, DateTime.Now));
        }

    }
}