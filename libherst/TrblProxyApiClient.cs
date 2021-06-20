using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using libherst.Models;

namespace libherst
{
    public class TrblProxyApiClient : TrblApiClient
    {
        public async Task<bool> UploadProxyList(string path)
        {
            var response = await HttpClient.PostAsync($"{ENDPOINT}/proxy", await ToFileUpload(path));
            return response.StatusCode == HttpStatusCode.OK;
        }

        public async Task<BlockingCollection<Proxy>> GetProxiesAsync()
        {
            var json = await HttpClient.GetStringAsync($"{ENDPOINT}/proxy?working=true");
            var proxies = JsonSerializer.Deserialize<IEnumerable<Proxy>>(json, SerializerOptions);
            var bc = new BlockingCollection<Proxy>();

            if (proxies != null)
            {
                foreach (var proxy in proxies)
                    bc.Add(proxy);
            }
            return bc;
        }
        public async Task<BlockingCollection<Proxy>> GetProxiesAsync(string country)
        {
            var json = await HttpClient.GetStringAsync($"{ENDPOINT}/proxy?working=true?country={country}");
            var proxies = JsonSerializer.Deserialize<IEnumerable<Proxy>>(json, SerializerOptions);
            var bc = new BlockingCollection<Proxy>();

            if (proxies != null)
            {
                foreach (var proxy in proxies)
                    bc.Add(proxy);
            }
            return bc;
        }

        public async Task<BlockingCollection<Proxy>> GetProxyToTest(int amount)
        {
            var json = await HttpClient.GetStringAsync($"{ENDPOINT}/proxy/test?count={amount}");
            var proxies = JsonSerializer.Deserialize<IEnumerable<Proxy>>(json, SerializerOptions).ToArray();
            var bc = new BlockingCollection<Proxy>(proxies.Length);
            foreach(var proxy in proxies)
                bc.Add(proxy);
            return bc;
        }

        public async Task<bool> UpdateProxyAsync(Proxy proxy)
        {
            var response = await HttpClient.PatchAsync($"{ENDPOINT}/proxy", ToJsonContent(proxy));
            return response.StatusCode == HttpStatusCode.OK;
        }
        public async Task<bool> DeleteProxyAsync(Proxy proxy)
        {
            var response = await HttpClient.DeleteAsync($"{ENDPOINT}/proxy?id=" + proxy.Id);
            return response.StatusCode == HttpStatusCode.OK;
        }
    }
}