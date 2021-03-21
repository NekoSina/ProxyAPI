using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProxyTester
{
    class Program
    {
        public const string USER="trbl";
        public const string PASS="herst";
        static async Task Main(string[] args)
        {
            var prxyClient = new ProxyTestClient();
            var trblClient = new TrblApiClient();

            if (await trblClient.Login(USER,PASS))
            {
                var proxies = await trblClient.GetProxiesAsync();
                Parallel.ForEach(proxies.OrderByDescending(p => p.KnownAsProxy), async proxy =>
                {
                    await prxyClient.Test(proxy,TimeSpan.FromSeconds(10));
                    await trblClient.UpdateProxy(proxy);
                });
                Console.WriteLine("Proxies: " + proxies.Length);
            }
        }
    }
}