using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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

            if (await trblClient.LoginAsync(USER, PASS))
            {
                Console.WriteLine($"Logged in as {USER}!");

                //if(await trblClient.UploadProxyList("/home/alu/thelist.txt"))
                //    Console.WriteLine("Uploaded!");
                while (true)
                {
                    var threadCount = Environment.ProcessorCount*8;
                    var taskList = new List<Task>();
                    var proxies = await trblClient.GetProxiesAsync();
                    var current = 0;
                    var total = proxies.Count;
                    Console.WriteLine($"Got {total} proxies to test, starting work on {threadCount} threads...");
                    Thread.Sleep(3000);
                    for (int i = 0; i < threadCount; i++)
                    {
                        var task = Task.Run(async () =>
                        {
                            foreach (var proxy in proxies.GetConsumingEnumerable())
                            {
                                Console.Title = $"Working on Proxy [{proxy.Id}] - {proxy.IP}:{proxy.Port} ({current}/{total})";
                                Interlocked.Increment(ref current);
                                await prxyClient.TestAsync(proxy, TimeSpan.FromSeconds(30));
                                await trblClient.UpdateProxyAsync(proxy);

                                if (proxy.Score < -100 && !proxy.Working)
                                    if (await trblClient.DeleteProxyAsync(proxy))
                                        Console.WriteLine($"Deleted {proxy.Id} because Score was {proxy.Score}");

                                if (proxies.Count == 0)
                                    break;
                            }
                        });

                        taskList.Add(task);
                    }
                    await Task.WhenAll(taskList);
                }
            }
        }
    }
}