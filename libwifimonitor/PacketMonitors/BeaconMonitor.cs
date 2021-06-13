using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PacketDotNet.Ieee80211;
using libherst;
using libherst.Models;

namespace libwifimonitor.PacketMonitors
{
    public class BeaconMonitor : BaseMonitor<BeaconFrame>
    {
        public BeaconMonitor(string path) : base(path) { }

        public override async Task HandlePacket(RadioPacket packet, BeaconFrame bp)
        {
            var signal = packet[RadioTapType.DbmAntennaSignal];
            var ssidObj = bp.InformationElements.First(b => b.Id.ToString() == "ServiceSetIdentity");
            var ssid = Encoding.UTF8.GetString(ssidObj.Value, 0, ssidObj.ValueLength).Trim();

            if (string.IsNullOrWhiteSpace(ssid))
                return;

            var dbs = signal.ToString().Split(' ',StringSplitOptions.TrimEntries)[1];
            var db = int.Parse(dbs);
            var output = $"{DateTime.Now},Beacon,{bp.SourceAddress},{ssid},{Helper.DbToPercent(db)}";
            if (BeaconCache.UpdateCache(bp.SourceAddress.ToString(),ssid, Helper.DbToPercentInt(db)))
            {
                var mac = new WiFiMac { MAC = bp.SourceAddress.ToString() };
                var ap = new WiFiAccessPoint
                {
                    LastSeen = DateTime.Now,
                    WiFiMac = mac,
                    WiFiNetworkName = new WiFiNetworkName { SSID = ssid }
                };
                ApiQueue.QueueItem(ap);
                Log(output);
            }
        }
    }
}