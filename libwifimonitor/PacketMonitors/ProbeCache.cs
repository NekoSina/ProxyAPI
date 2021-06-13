using System;
using System.Collections.Generic;
using System.Linq;

namespace libwifimonitor
{
    public static class ProbeCache
    {
        public static Dictionary<string, List<CacheEntry>> Cache = new();

        public static CacheEntry GetCacheEntry(string mac, string ssid)
        {
            if (!Cache.TryGetValue(ssid, out var list)) 
                return null;
            return list.FirstOrDefault(ce => ce.SSID == ssid && ce.MAC == mac);
        }

        public static bool UpdateCache(string mac, string ssid, int signal)
        {
            var cacheEntry = new CacheEntry
            {
                LastSeen=DateTime.Now,
                MAC = mac,
                SSID = ssid,
                SeenTimes = 1,
                SignalStrength = signal,
                LastSignalStrength = -1,
            };

            if (!Cache.TryGetValue(ssid, out var list))
                return Cache.TryAdd(ssid, new List<CacheEntry> {cacheEntry});
            
            var ce = list.FirstOrDefault(ce => ce.MAC == mac);
            if(ce !=null)
            {
                ce.LastSeen=DateTime.Now;
                ce.SeenTimes++;
                ce.LastSignalStrength = ce.SignalStrength;
                ce.SignalStrength = signal;
                return false;
            }

            list.Add(cacheEntry);
            return true;

        }
    }
}