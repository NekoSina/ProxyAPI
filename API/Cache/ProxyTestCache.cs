using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using libherst.Models;

namespace HerstAPI.Cache
{
    public static class ProxyTestCache
    {
        class CacheEntry
        {
            public Proxy proxy;
            public DateTime Expiration;
        }

        private static readonly Thread cacheInvalidationThread;
        private static readonly ConcurrentDictionary<ulong, CacheEntry> Cache = new();

        static ProxyTestCache()
        {
            cacheInvalidationThread = new (WorkLoop);
            cacheInvalidationThread.IsBackground=true;
            cacheInvalidationThread.Start();
        }

        private static void WorkLoop(object obj)
        {
            var invalidated = new List<ulong>();
            while(true)
            {
                invalidated.Clear();

                foreach(var kvp in Cache)
                    if(DateTime.Now > kvp.Value.Expiration)
                        invalidated.Add(kvp.Key);

                foreach(var key in invalidated)
                    Cache.TryRemove(key, out _);

                Thread.Sleep(TimeSpan.FromMinutes(5));
            }
        }

        public static void Add(Proxy proxy) => Cache.TryAdd(proxy.Id, new CacheEntry { proxy = proxy, Expiration = DateTime.Now.AddMinutes(15) });
        public static void Remove(ulong id) => Cache.TryRemove(id, out _);
        internal static bool Contains(ulong id) => Cache.ContainsKey(id);
    }
}