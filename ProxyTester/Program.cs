using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProxyTester.Models;

namespace ProxyTester
{
    class Program
    {
        public const string USER = "trbl";
        public const string PASS = "herst";
        static async Task Main(string[] args)
        {
            var prxyClient = new ProxyTestClient();
            var trblClient = new TrblApiClient();

            if (await trblClient.Login(USER, PASS))
            {
                Console.WriteLine($"Logged in as {USER}!");

                //if(await trblClient.UploadProxyList("/home/alu/hugelist.txt"))
                //    Console.WriteLine("Uploaded!");

                var proxies = await trblClient.GetProxiesAsync();
                var bc = new BlockingCollection<Proxy>();

                foreach (var proxy in proxies)
                    bc.Add(proxy);

                var taskList = new List<Task>();
                for (int i = 0; i < Environment.ProcessorCount * 2; i++)
                {
                    var task = Task.Run(async () =>
                    {
                        foreach (var proxy in bc.GetConsumingEnumerable())
                        {
                            await prxyClient.Test(proxy, TimeSpan.FromSeconds(3));
                            await trblClient.UpdateProxy(proxy);

                            if (proxy.Score < -100 && !proxy.Working)
                                if (await trblClient.DeleteProxy(proxy))
                                    Console.WriteLine($"Deleted {proxy.Id} because Score was {proxy.Score}");

                            if (bc.Count == 0)
                                break;
                        }
                    });
                    
                    taskList.Add(task);
                }
                await Task.WhenAll(taskList);
                Console.WriteLine("Proxies: " + proxies.Length);
            }
        }
    }
}