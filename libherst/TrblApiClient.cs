using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using libherst.Models;

namespace libherst
{
    ///
    /// <Summary>Instantiate TrblProxyClient, TrblWifiClient,... instead</Summary>
    public abstract class TrblApiClient
    {
        internal const string ENDPOINT = "http://localhost:5000/api";
        internal readonly JsonSerializerOptions SerializerOptions;
        internal readonly HttpClient HttpClient;

        public TrblApiClient()
        {
            HttpClient = new HttpClient {Timeout = TimeSpan.FromSeconds(10)};
            SerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, };
        }

        public async Task<bool> LoginAsync(string user, string pass)
        {
            var creds = new UserInfo { Username = user ?? "trbl", Password = pass ?? "herst" };
            var response = await HttpClient.PostAsync($"{ENDPOINT}/auth/token", ToJsonContent(creds));
            var token = await response.Content.ReadAsStringAsync();

            HttpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            return response.StatusCode == HttpStatusCode.OK;
        }

        internal static async Task<HttpContent> ToFileUpload(string path)
        {
            using var form = new MultipartFormDataContent();
            using var fileContent = new ByteArrayContent(await File.ReadAllBytesAsync(path));
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
            form.Add(fileContent, "file", Path.GetFileName(path));
            return form;
        }

        internal static StringContent ToJsonContent(object input) => new(JsonSerializer.Serialize(input), Encoding.UTF8, "application/json");
    }
}