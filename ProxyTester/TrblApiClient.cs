using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ProxyTester.Models;

namespace ProxyTester
{
    public class TrblApiClient
    {
        const string ENDPOINT = "http://127.0.0.1:5000/api";
        private JsonSerializerOptions serializerOptions;
        readonly HttpClient httpClient;

        public TrblApiClient()
        {
            httpClient = new HttpClient();
            serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, };
        }

        public async Task<bool> Login(string user, string pass)
        {
            var creds = new UserInfo { Username = "trbl", Password = "herst" };
            using var content = new StringContent(JsonSerializer.Serialize(creds), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync($"{ENDPOINT}/Token", content);
            var token = await response.Content.ReadAsStringAsync();

            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            return response.StatusCode == HttpStatusCode.OK;
        }

        public async Task<bool> UploadProxyList(string path)
        {
            using var form = new MultipartFormDataContent();
            using var fileContent = new ByteArrayContent(await File.ReadAllBytesAsync(path));
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
            form.Add(fileContent, "file", Path.GetFileName(path));
            var response = await httpClient.PostAsync($"{ENDPOINT}/proxy", form);
            return response.StatusCode == HttpStatusCode.OK;
        }
        public async Task<Proxy[]> GetProxiesAsync()
        {
            var json = await httpClient.GetStringAsync($"{ENDPOINT}/proxy?score=0");
            var proxies = JsonSerializer.Deserialize<Proxy[]>(json, serializerOptions);
            return proxies;
        }
        public async Task<bool> UpdateProxy(Proxy proxy)
        {
            using var content = new StringContent(JsonSerializer.Serialize(proxy), Encoding.UTF8, "application/json");
            var putResponse = await httpClient.PatchAsync($"{ENDPOINT}/proxy", content);
            return putResponse.StatusCode == HttpStatusCode.OK;
        }
        public async Task<bool> DeleteProxy(Proxy proxy)
        {
            var response = await httpClient.DeleteAsync($"{ENDPOINT}/proxy?id=" + proxy.Id);
            return response.StatusCode == HttpStatusCode.OK;
        }
    }
}