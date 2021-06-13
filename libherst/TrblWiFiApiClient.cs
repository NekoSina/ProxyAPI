using System;
using System.Net;
using System.Threading.Tasks;
using libherst.Models;

namespace libherst
{
    public class TrblWiFiApiClient : TrblApiClient
    {
        public async Task<bool> SubmitProbe(WiFiProbe probe)
        {
            try
            {
                var response = await HttpClient.PutAsync($"{ENDPOINT}/wifi/probe",ToJsonContent(probe));
                if(response.StatusCode != HttpStatusCode.OK)
                    return false;
                var dto = await response.Content.ReadAsStringAsync();
                Console.WriteLine(dto);
                return true;
            }
            catch { return false; }
        }
        public async Task<bool> SubmitBeacon(WiFiAccessPoint ap)
        {
            try
            {
                var response = await HttpClient.PutAsync($"{ENDPOINT}/wifi/accesspoint",ToJsonContent(ap));
                if(response.StatusCode != HttpStatusCode.OK)
                    return false;
                var result = await response.Content.ReadAsStringAsync();
                Console.WriteLine(result);
                return true;
            }
            catch { return false; }
        }
    }
}