using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using libherst.Models;
using MihaZupan;

namespace ProxyTester
{
    public static class ProxyTestClient
    {
        private const string HTTPS_TEST_TARGET = "https://her.st/";
        private const string HTTP_TEST_TARGET = "http://her.st/";
        private static readonly string _httpsResponseWithoutProxy = new WebClient().DownloadString(HTTPS_TEST_TARGET).Trim();
        private static readonly string _httpResponseWithoutProxy = new WebClient().DownloadString(HTTPS_TEST_TARGET).Trim();

        public static async Task<bool> TestAsync(Proxy proxy, TimeSpan timeout)
        {
            proxy.LastTest = DateTime.UtcNow;

            if (!await HttpsTest(proxy, timeout))
            {
                if(!await HttpTest(proxy,timeout))
                {
                    if (!await Socks5HttpsTest(proxy, timeout))
                    {
                        if (!await Socks5HttpTest(proxy, timeout))
                        {
                            proxy.Type= "UNKNOWN";
                            proxy.Score--;
                            proxy.Working = false;
                        }
                        else
                        {
                            proxy.Type = "SOCKS5/HTTP";
                            proxy.Score += 3;
                            proxy.Working = true;
                        }
                    }
                    else
                    {
                        proxy.Type = "SOCKS5/HTTPS";
                        proxy.Score += 3;
                        proxy.Working = true;
                    }
                }
                else
                {
                    proxy.Type = "HTTP";
                    proxy.Score += 3;
                    proxy.Working = true;
                }
            }
            else
            {
                proxy.Type = "HTTPS";
                proxy.Score += 3;
                proxy.Working = true;
            }
            return proxy.Working;
        }
        public static async Task<bool> HttpsTest(Proxy proxy, TimeSpan timeout)
        {
            var webProxy = new WebProxy(proxy.IP.ToString(), proxy.Port);
            var request = (HttpWebRequest)WebRequest.Create(HTTPS_TEST_TARGET);
            request.Proxy = webProxy;
            request.Timeout = (int)timeout.TotalMilliseconds;

            try
            {
                var resp = await request.GetResponseAsync();
                var reader = new StreamReader(resp.GetResponseStream());
                var html = await reader.ReadToEndAsync();
                Console.WriteLine($"[HTTPS][{proxy.Id}] working");
                return html.Trim() == _httpResponseWithoutProxy;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[HTTPS][{proxy.Id}] {e.Message}");
            }
            return false;
        }
        public static async Task<bool> HttpTest(Proxy proxy, TimeSpan timeout)
        {
            var webProxy = new WebProxy(proxy.IP.ToString(), proxy.Port);
            var request = (HttpWebRequest)WebRequest.Create(HTTP_TEST_TARGET);
            request.Proxy = webProxy;
            request.Timeout = (int)timeout.TotalMilliseconds;

            try
            {
                var resp = await request.GetResponseAsync();
                var reader = new StreamReader(resp.GetResponseStream());
                var html = await reader.ReadToEndAsync();
                Console.WriteLine($"[HTTP][{proxy.Id}] working");
                return html.Trim() == _httpResponseWithoutProxy;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[HTTP][{proxy.Id}] {e.Message}");
            }
            return false;
        }
        public static async Task<bool> Socks5HttpsTest(Proxy proxy, TimeSpan timeout)
        {
            var webProxy = new HttpToSocks5Proxy(proxy.IP, proxy.Port);
            //webProxy.Timeout = TimeSpan.FromSeconds(timeout);
            var handler = new HttpClientHandler { Proxy = webProxy };
            HttpClient httpClient = new HttpClient(handler, true);

            try
            {
                var result = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "https://her.st/"));
                var html = await result.Content.ReadAsStringAsync();
                Console.WriteLine($"[SOCKS5/HTTPS][{proxy.Id}] working");
                return html.Trim() == _httpsResponseWithoutProxy;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[SOCKS5/HTTPS][{proxy.Id}] {e.Message}");
            }
            return false;
        }
        public static async Task<bool> Socks5HttpTest(Proxy proxy, TimeSpan timeout)
        {
            var webProxy = new HttpToSocks5Proxy(proxy.IP, proxy.Port);
            //webProxy.Timeout = TimeSpan.FromSeconds(timeout);
            var handler = new HttpClientHandler { Proxy = webProxy };
            HttpClient httpClient = new HttpClient(handler, true);

            try
            {
                var result = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://her.st/"));
                var html = await result.Content.ReadAsStringAsync();
                Console.WriteLine($"[SOCKS5/HTTP][{proxy.Id}] working");
                return html.Trim() == _httpResponseWithoutProxy;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[SOCKS5/HTTP][{proxy.Id}] {e.Message}");
            }
            return false;
        }
    }
}