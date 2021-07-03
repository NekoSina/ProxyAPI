using HerstAPI.Repositories;
using System;
using System.Threading;
using System.Collections.Concurrent;
using HerstAPI.Database;
using libherst.Models;

namespace HerstAPI.Logic
{
    public static class ProxyListCruncher
    {
        private static readonly ConcurrentDictionary<string, object> _cache = new();
        private static readonly IP2Location.Component IpQuery = new();
        private static readonly IP2Proxy.Component ProxyQuery = new();
        private static ulong processedLines = 0;
        private static readonly BlockingCollection<string> pendingLines = new();
        private static readonly Thread[] workerThreads;

        static ProxyListCruncher()
        {
            IpQuery.Open("Assets/ipdb.bin", true);
            IpQuery.LoadBIN();
            ProxyQuery.Open("Assets/proxydb.bin", IP2Proxy.Component.IOModes.IP2PROXY_MEMORY_MAPPED);
            workerThreads = new Thread[Environment.ProcessorCount];
            for (int i = 0; i < workerThreads.Length; i++)
            {
                workerThreads[i] = new Thread(WorkLoop);
                workerThreads[i].Start();
            }
        }

        private static void WorkLoop()
        {
            var repo = new ProxyRepository(new HerstDbContext());
            foreach (var line in pendingLines.GetConsumingEnumerable())
            {
                try
                {
                    var parts = line.Split(':');
                    var ip = parts[0];
                    var port = ushort.Parse(parts[1]);

                    var result = IpQuery.IPQuery(ip);
                    var proxyResult = ProxyQuery.GetAll(ip);
                    
                    switch (proxyResult.Usage_Type)
                    {
                        case "COM": proxyResult.Usage_Type = "Commercial"; break;
                        case "ORG": proxyResult.Usage_Type = "Organization"; break;
                        case "GOV": proxyResult.Usage_Type = "Government"; break;
                        case "MIL": proxyResult.Usage_Type = "Military"; break;
                        case "EDU": proxyResult.Usage_Type = "University/College/School"; break;
                        case "LIB": proxyResult.Usage_Type = "Library"; break;
                        case "CDN": proxyResult.Usage_Type = "Content Delivery Network"; break;
                        case "ISP": proxyResult.Usage_Type = "Fixed Line ISP"; break;
                        case "MOB": proxyResult.Usage_Type = "Mobile ISP"; break;
                        case "ISP/MOB": proxyResult.Usage_Type = "Fixed/Mobile ISP"; break;
                        case "DCH": proxyResult.Usage_Type = "Data Center/Web Hosting/Transit"; break;
                        case "SES": proxyResult.Usage_Type = "Search Engine Spider"; break;
                        case "-": proxyResult.Usage_Type = ""; break;
                    }

                    var proxy = new Proxy
                    {
                        Id = Helpers.IpToInt(ip),
                        IP = ip,
                        Port = port,
                        City = result.City,
                        Country = result.CountryLong,
                        Region = result.Region,
                        Longitude = result.Longitude,
                        Latitude = result.Latitude,
                        AS = proxyResult.AS,
                        ASN = proxyResult.ASN,
                        Domain = proxyResult.Domain,
                        ISP = proxyResult.ISP,
                        Threat = proxyResult.Threat,
                        ProxyType = proxyResult.Usage_Type
                    };

                    foreach (var m in proxy.GetType().GetProperties())
                    {
                        if (m.PropertyType != typeof(String))
                            continue;

                        var val = (string)m.GetValue(proxy);

                        if (val != "-")
                            continue;

                        m.SetValue(proxy, string.Empty);
                        val = (string)m.GetValue(proxy);
                    }

                    repo.TryAddProxy(proxy);
                    _cache.TryRemove(ip, out _);
                    Interlocked.Increment(ref processedLines);
                    Console.WriteLine($"Processed: {processedLines}, Pending:{pendingLines.Count}");
                }
                catch
                {

                }
            }
        }
        private static bool CheckIfProxy(string proxy, out string ip, out ushort port)
        {
            ip = "127.0.0.1";
            port = 0;

            if (proxy.Contains("["))
                return false;

            int dotcount = proxy.Length - proxy.Replace(".", "").Length;

            if (dotcount != 3)
                return false;

            int coloncount = proxy.Length - proxy.Replace(":", "").Length;

            if (coloncount != 1)
                return false;

            if (proxy.Contains("("))
                proxy = proxy[proxy.IndexOf("(")..].Replace(")", "").Trim();

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

        public static void Enqueue(string line)
        {
            if (!CheckIfProxy(line, out var ip, out var port))
            {
                Console.WriteLine("Invalid line in list: " + line);
                return;
            }
            if (_cache.ContainsKey(ip))
            {
                Console.WriteLine("Duplicate line in list: " + line);
                return;
            }

            _cache.TryAdd(ip, null);
            pendingLines.Add($"{ip}:{port}");
        }
    }
}