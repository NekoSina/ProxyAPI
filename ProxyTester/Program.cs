using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProxyTester
{
    public class Proxy
    {
        public ulong Id { get; set; }

        public string IP { get; set; }
        public ushort Port { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public float Longitude { get; set; }
        public float Latitude { get; set; }
        public DateTime LastTest { get; set; }
        public string AS { get; set; }
        public string ASN { get; set; }
        public string Domain { get; set; }
        public string ISP { get; set; }
        public bool KnownAsProxy => !string.IsNullOrEmpty(AS);
        public DateTime LastSeen { get; set; }
        public string ProxyType { get; set; }
        public string Threat { get; set; }

        public override string ToString() => $"{IP}:{Port} - {Region}, {Country}, {City}";
    }

    public class UserInfo
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    class Program
    {
        private static string _responseWithoutProxy = new WebClient().DownloadString("https://her.st").Trim();
        static async Task Main(string[] args)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            var creds = new UserInfo { Username = "trbl", Password = "herst" };
            var httpCli = new HttpClient();
            var content = new StringContent(JsonSerializer.Serialize(creds), Encoding.UTF8, "application/json");
            var response = await httpCli.PostAsync("http://127.0.0.1:5000/api/Token", content);
            var token = await response.Content.ReadAsStringAsync();
            httpCli.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            var proxies = await httpCli.GetStringAsync($"http://127.0.0.1:5000/api/proxy");
            var proxyList = JsonSerializer.Deserialize<Proxy[]>(proxies, options);

            Parallel.ForEach(proxyList.OrderByDescending(p => p.KnownAsProxy), async proxy =>
              {
                  bool working = false;
                  var reason = "";
                  proxy.LastTest = DateTime.UtcNow;
                  for (int i = 0; i < 3; i++)
                  {
                      try
                      {
                          var webProxy = new WebProxy(proxy.IP.ToString(), proxy.Port);
                          var request = (HttpWebRequest)WebRequest.Create("https://her.st");
                          request.Proxy = webProxy;
                          request.Timeout = (int)TimeSpan.FromSeconds(15).TotalMilliseconds;
                          var resp = request.GetResponse();
                          var reader = new StreamReader(resp.GetResponseStream());
                          var html = reader.ReadToEnd().Trim();

                          if (html == _responseWithoutProxy)
                          {
                              Console.WriteLine("Proxy working: " + proxy.Id);
                              proxy.LastSeen = DateTime.UtcNow;
                              content = new StringContent(JsonSerializer.Serialize(proxy), Encoding.UTF8, "application/json");
                              var putResponse = await httpCli.PatchAsync("http://127.0.0.1:5000/api/proxy", content);
                              working=true;
                              break;
                          }
                      }
                      catch (Exception e)
                      {
                          reason = e.Message;
                          if (e.Message.Contains("denied", StringComparison.InvariantCultureIgnoreCase))
                              break;
                          if (e.Message.Contains("access", StringComparison.InvariantCultureIgnoreCase))
                              break;
                          if (e.Message.Contains("refused", StringComparison.InvariantCultureIgnoreCase))
                              break;
                          if (e.Message.Contains("forbidden", StringComparison.InvariantCultureIgnoreCase))
                              break;
                      }
                  }
                  if (!working)
                  {
                      Console.WriteLine("Proxy dead: " + proxy.Id + " " + reason);
                      content = new StringContent(JsonSerializer.Serialize(proxy), Encoding.UTF8, "application/json");
                      await httpCli.DeleteAsync("http://127.0.0.1:5000/api/proxy?id=" + proxy.Id);
                  }
              });
            Console.WriteLine("Proxies: " + proxyList.Length);
        }
    }
}