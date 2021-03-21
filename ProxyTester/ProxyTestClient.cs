using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using ProxyTester.Models;

namespace ProxyTester
{
    public class ProxyTestClient
    {
        private const string TEST_TARGET = "https://her.st/";
        private static string _responseWithoutProxy = new WebClient().DownloadString(TEST_TARGET).Trim();

        public async Task<bool> Test(Proxy proxy, TimeSpan timeout)
        {
            proxy.LastTest = DateTime.UtcNow;
            var working = false;
            try
            {
                var webProxy = new WebProxy(proxy.IP.ToString(), proxy.Port);
                var request = (HttpWebRequest)WebRequest.Create(TEST_TARGET);
                request.Proxy = webProxy;
                request.Timeout = (int)timeout.TotalMilliseconds;
                var resp = await request.GetResponseAsync();
                var reader = new StreamReader(resp.GetResponseStream());
                var html = await reader.ReadToEndAsync();

                return html.Trim() == _responseWithoutProxy;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[{proxy.Id}] {e.Message}");
            }
            return working;
        }
    }
}