using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using libherst;

namespace ProxyTester
{
    class Program
    {
        public static string USER = "demo";
        public static string PASS = "demo";
        public static int THREAD_COUNT = 8;

        static async Task Main(string[] args)
        {
            USER=args[0];
            PASS=args[1];
            THREAD_COUNT= int.Parse(args[2]);

            var prxyClient = new ProxyTestClient();
            var trblClient = new TrblProxyApiClient();

            if (await trblClient.LoginAsync(USER, PASS))
            {
                Console.WriteLine($"Logged in as {USER}!");

                while (true)
                {
                    var taskList = new List<Task>();
                    var proxies = await trblClient.GetProxiesAsync();
                    var current = 0;
                    var total = proxies.Count;
                    Console.WriteLine($"Got {total} proxies to test, starting work on {THREAD_COUNT} threads...");
                    Thread.Sleep(3000);
                    for (int i = 0; i < THREAD_COUNT; i++)
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