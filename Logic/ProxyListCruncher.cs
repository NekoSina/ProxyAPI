using ProxyAPI.Models;
using ProxyAPI.Repositories;
using ProxyAPI.IP2Location;
using System.Net;
using System;
using System.Threading;
using System.Collections.Concurrent;
using ProxyAPI.Database;

namespace ProxyAPI.Logic
{
    public static class ProxyListCruncher
    {
        private static IpLocator Locator = new BinaryDbClient("ipdb.bin");
        private static ulong processedLines = 0;
        private static BlockingCollection<string> pendingLines = new BlockingCollection<string>();
        private static Thread[] workerThreads;

        static ProxyListCruncher()
        {
            workerThreads = new Thread[Environment.ProcessorCount*2];
            for (int i = 0; i < workerThreads.Length; i++)
            {
                workerThreads[i] = new Thread(WorkLoop);
                workerThreads[i].Start();
            }
        }

        private static void WorkLoop()
        {
            var repo = new ProxyRepository(new ProxyDbContext());
            foreach (var line in pendingLines.GetConsumingEnumerable())
            {
                if (CheckIfProxy(line, out var ip, out var port))
                {
                    var location = Locator.Locate(IPAddress.Parse(ip));
                    var proxy = new Proxy
                    {
                        ID = Helpers.IpToInt(ip),
                        IP = ip,
                        Port = port,
                        City = location.City,
                        Country = location.Country,
                        Region = location.Region
                    };
                    
                    repo.AddOrUpdateProxy(proxy);
                }
                
                Interlocked.Increment(ref processedLines);
                
                if(pendingLines.Count <2)
                    repo.Save();
                
                Console.WriteLine($"Processed: {processedLines}, Pending:{pendingLines.Count}");
            }
        }
        private static bool CheckIfProxy(string proxy, out string ip, out ushort port)
        {
            ip = "127.0.0.1";
            port = 0;

            int dotcount = proxy.Length - proxy.Replace(".", "").Length;

            if (dotcount != 3)
                return false;

            int coloncount = proxy.Length - proxy.Replace(":", "").Length;

            if (coloncount != 1)
                return false;

            var proxyParts = proxy.Split(':');
            var portString = proxyParts[1];

            if (!ushort.TryParse(portString, out port))
                return false;

            var ipParts = proxyParts[0].Split('.');

            foreach (var part in ipParts)
                if (!byte.TryParse(part, out _))
                    return false;

            ip = proxyParts[0];

            return true;
        }

        public static void Enqueue(string line) => pendingLines.Add(line);
    }
}