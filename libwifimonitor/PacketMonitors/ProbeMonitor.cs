using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libherst;
using libherst.Models;
using PacketDotNet.Ieee80211;

namespace libwifimonitor
{
    public class ProbeMonitor : BaseMonitor<ProbeRequestFrame>
    {
        public ProbeMonitor(string path) : base(path) { }

        public override async Task HandlePacket(RadioPacket packet, ProbeRequestFrame pr)
        {
            var signal = packet[RadioTapType.DbmAntennaSignal];
            var ssidObj = pr.InformationElements.First(ie => ie.Id.ToString() == "ServiceSetIdentity");
            var ssid = Encoding.UTF8.GetString(ssidObj.Value, 0, ssidObj.ValueLength).Trim();

            if (string.IsNullOrWhiteSpace(ssid))
                return;
            var dbs = signal.ToString().Split(' ', StringSplitOptions.TrimEntries)[1];
            var db = int.Parse(dbs);
            var output = $"{DateTime.Now},Probe,{pr.SourceAddress},{ssid},{Helper.DbToPercent(db)}";
            
            if (ProbeCache.UpdateCache(ssid, pr.SourceAddress.ToString(),Helper.DbToPercentInt(db)))
            {
                var mac = new WiFiMac { MAC = pr.SourceAddress.ToString() };
                var probe = new WiFiProbe
                {
                    LastSeen = DateTime.Now,
                    WiFiMac = mac,
                    WiFiNetworkName = new WiFiNetworkName { SSID = ssid }
                };
                ApiQueue.QueueItem(probe);
                Log(output);
            }
        }
    }
}