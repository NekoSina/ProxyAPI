using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using libherst.Models;

namespace ProxyTester
{
    public static class ProxyTestClient
    {
        private const string TEST_TARGET = "https://her.st/";
        private static readonly string _responseWithoutProxy = new WebClient().DownloadString(TEST_TARGET).Trim();

        public static async Task<bool> TestAsync(Proxy proxy, TimeSpan timeout)
        {
            proxy.LastTest = DateTime.UtcNow;

            var webProxy = new WebProxy(proxy.IP.ToString(), proxy.Port);
            var request = (HttpWebRequest)WebRequest.Create(TEST_TARGET);
            request.Proxy = webProxy;
            request.Timeout = (int)timeout.TotalMilliseconds;

            try
            {
                var resp = await request.GetResponseAsync();
                var reader = new StreamReader(resp.GetResponseStream());
                var html = await reader.ReadToEndAsync();

                proxy.Working = html.Trim() == _responseWithoutProxy;
                
                if (proxy.Working)
                    proxy.Score += 3;
            }
            catch (Exception e)
            {
                proxy.Score--;
                proxy.Working = false;
                Console.WriteLine($"[{proxy.Id}] {e.Message}");
            }
            return proxy.Working;
        }
    }
}