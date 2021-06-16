using System;
using PacketDotNet.Ieee80211;
using SharpPcap;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using libherst.Models;
using libwifimonitor.PacketMonitors;
using libwifimonitor;

namespace wifimon
{
    class Program
    {

        public static BeaconMonitor BeaconMonitor;
        public static ProbeMonitor ProbeMonitor;
        public static ICaptureDevice Device;


        public static string Color(int color) => $"\u001b[38;5;{color}m";
        static async Task Main()
        {
            // for (int x = 0; x < 16; x++)
            // {
            //     for (int y = 0; y < 16; y++)
            //     {
            //         var code = x * 16 + y;
            //         Console.Write($"{Color(code)}{code:000}");
            //     }
            //     Console.WriteLine();
            // }

            // Console.ReadLine();
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            Device = FindDevice();
            try
            {
                await MonitoringMode(true);
                Console.WriteLine($"-- Listening on { Device.Name}, hit 'Enter' to stop...");
                var sb = new StringBuilder();
                var probes = new Dictionary<CacheEntry, string>();
                var aps = new List<CacheEntry>();
                Console.Clear();
                while (true)
                {
                    //Console.Clear();
                    sb.Clear();
                    aps.Clear();
                    probes.Clear();

                    foreach (var kvp in BeaconCache.Cache)
                    {
                        foreach (var entry in kvp.Value)
                        {
                            if (DateTime.Now - entry.LastSeen < TimeSpan.FromMinutes(5))
                                aps.Add(entry);
                        }
                    }
                    foreach (var kvp in ProbeCache.Cache)
                    {
                        var first = kvp.Value.FirstOrDefault();

                        if (DateTime.Now - first.LastSeen > TimeSpan.FromMinutes(5))
                            continue;
                        var ssids = "";
                        foreach (var entry in kvp.Value)
                            ssids += entry.SSID + ", ";
                        probes.TryAdd(first, ssids);
                    }

                    sb.AppendLine("############################# PROBES #########################################");
                    sb.AppendLine($"| Last seen | Count | Signal |                         MAC                   |");
                    foreach (var kvp in probes.OrderByDescending(p => p.Key.SignalStrength))
                    {
                        var a = kvp.Key.LastSeen.ToString("HH:mm:ss");
                        var b = $"{kvp.Key.SeenTimes:000}";
                        var c = $"{kvp.Key.SignalStrength:000}";
                        var d = kvp.Key.MAC;
                        var e = kvp.Value;

                        var colorCode = 40;
                        if (DateTime.Now - kvp.Key.LastSeen > TimeSpan.FromSeconds(5))
                            colorCode = 41;
                        if (DateTime.Now - kvp.Key.LastSeen > TimeSpan.FromSeconds(15))
                            colorCode = 42;
                        if (DateTime.Now - kvp.Key.LastSeen > TimeSpan.FromSeconds(30))
                            colorCode = 43;
                        if (DateTime.Now - kvp.Key.LastSeen > TimeSpan.FromSeconds(45))
                            colorCode = 44;
                        if (DateTime.Now - kvp.Key.LastSeen > TimeSpan.FromMinutes(1))
                            colorCode = 62;
                        if (DateTime.Now - kvp.Key.LastSeen > TimeSpan.FromMinutes(2))
                            colorCode = 56;
                        if (DateTime.Now - kvp.Key.LastSeen > TimeSpan.FromMinutes(3))
                            colorCode = 54;
                        if (DateTime.Now - kvp.Key.LastSeen > TimeSpan.FromMinutes(4))
                            colorCode = 90;

                        a = Color(colorCode) + a + Color(7);

                        if (kvp.Key.LastSignalStrength - kvp.Key.SignalStrength < -2)
                            c = Color(40) + c + '+' + Color(7);
                        else if (kvp.Key.LastSignalStrength - kvp.Key.SignalStrength > 2)
                            c = Color(1) + c + '-' + Color(7);
                        else
                            c = Color(7) + c + ' ' + Color(7);

                        if (e.EndsWith(", "))
                            e = e.Remove(e.Length - 2);

                        sb.AppendLine($"| {a}  |  {b}  |  {c}  | {d} | {e} |");
                    }
                    sb.AppendLine("                                                                              ");
                    sb.AppendLine("############################# BEACONS ########################################");
                    sb.AppendLine($"| Last seen | Count | Signal |     MAC      |               SSID             |");

                    var newest = aps.OrderByDescending(p => p.SignalStrength).Take(25).OrderByDescending(p => p.SignalStrength);

                    foreach (var entry in newest)
                    {
                        var a = entry.LastSeen.ToString("HH:mm:ss");
                        var b = $"{entry.SeenTimes:000}";
                        var c = $"{entry.SignalStrength:000}";
                        var d = entry.MAC;
                        var e = entry.SSID.Trim().Trim(' ').PadRight(30);

                        var colorCode = 40;
                        if (DateTime.Now - entry.LastSeen > TimeSpan.FromSeconds(5))
                            colorCode = 41;
                        if (DateTime.Now - entry.LastSeen > TimeSpan.FromSeconds(15))
                            colorCode = 42;
                        if (DateTime.Now - entry.LastSeen > TimeSpan.FromSeconds(30))
                            colorCode = 43;
                        if (DateTime.Now - entry.LastSeen > TimeSpan.FromSeconds(45))
                            colorCode = 44;
                        if (DateTime.Now - entry.LastSeen > TimeSpan.FromMinutes(1))
                            colorCode = 62;
                        if (DateTime.Now - entry.LastSeen > TimeSpan.FromMinutes(2))
                            colorCode = 56;
                        if (DateTime.Now - entry.LastSeen > TimeSpan.FromMinutes(3))
                            colorCode = 54;
                        if (DateTime.Now - entry.LastSeen > TimeSpan.FromMinutes(4))
                            colorCode = 90;

                        a = Color(colorCode) + a + Color(7);

                        if (entry.LastSignalStrength - entry.SignalStrength < -2)
                            c = Color(40) + c + '+' + Color(7);
                        else if (entry.LastSignalStrength - entry.SignalStrength > 2)
                            c = Color(1) + c + '-' + Color(7);
                        else
                            c = Color(7) + c + ' ' + Color(7);

                        sb.AppendLine($"| {a}  |  {b}  |  {c}  | {d} | {e} |");
                    }
                    Console.SetCursorPosition(0, 0);
                    Console.WriteLine(sb);
                    Thread.Sleep(1000);
                }
            }
            catch (Exception e) { Console.WriteLine(e); }
            finally { await MonitoringMode(false); }
        }

        private static ICaptureDevice FindDevice()
        {
            foreach (var dev in CaptureDeviceList.Instance)
                if (dev.Name.Contains("wlan") || dev.Name.Contains("wlp"))
                    return dev;
            return null;
        }
        private static async void Device_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            try
            {
                var packet = (RadioPacket)e.Packet.GetPacket();
                var beaconFrame = packet.Extract<BeaconFrame>();
                var probeRequest = packet.Extract<ProbeRequestFrame>();

                if (beaconFrame != null)
                    await BeaconMonitor.HandlePacket(packet, beaconFrame);

                if (probeRequest != null)
                    await ProbeMonitor.HandlePacket(packet, probeRequest);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static async Task MonitoringMode(bool on = true)
        {
            if (on)
            {
                Console.WriteLine($"-- Enabling Monitor mode on {Device.Name}");
                var wifiOffResult = await CliWrap.Cli.Wrap("nmcli").WithArguments($"radio wifi off").WithValidation(CliWrap.CommandResultValidation.None).ExecuteAsync();
                Console.WriteLine($"-- Disabling the WiFi chip took {wifiOffResult.RunTime.TotalMilliseconds}ms");
                var wifiOnResult = await CliWrap.Cli.Wrap("nmcli").WithArguments($"radio wifi on").WithValidation(CliWrap.CommandResultValidation.None).ExecuteAsync();
                Console.WriteLine($"-- Enabling the WiFi chip took {wifiOnResult.RunTime.TotalMilliseconds}ms");
                var wwanOnResult = await CliWrap.Cli.Wrap("nmcli").WithArguments($"radio wwan on").WithValidation(CliWrap.CommandResultValidation.None).ExecuteAsync();
                Console.WriteLine($"-- Enabling the modem took {wwanOnResult.RunTime.TotalMilliseconds}ms");
                var lteUpResult = await CliWrap.Cli.Wrap("nmcli").WithArguments($"connection PublicIP up").WithValidation(CliWrap.CommandResultValidation.None).ExecuteAsync();
                Console.WriteLine($"-- Connecting to 4G took {lteUpResult.RunTime.TotalMilliseconds}ms");
                var devDownResult = await CliWrap.Cli.Wrap("ip").WithArguments($"link set {Device.Name} down").WithValidation(CliWrap.CommandResultValidation.None).ExecuteAsync();
                Console.WriteLine($"-- Taking down {Device.Name} took {devDownResult.RunTime.TotalMilliseconds}ms");
                var modeResult = await CliWrap.Cli.Wrap("iwconfig").WithArguments($"{Device.Name} mode Monitor").WithValidation(CliWrap.CommandResultValidation.None).ExecuteAsync();
                Console.WriteLine($"-- Switching {Device.Name} to Monitor Mode took {modeResult.RunTime.TotalMilliseconds}ms");
                var setMacResult = await CliWrap.Cli.Wrap("ip").WithArguments($"sudo ip link set dev {Device.Name} address DE:AD:BE:EF:13:37").WithValidation(CliWrap.CommandResultValidation.None).ExecuteAsync();
                Console.WriteLine($"-- Setting {Device.Name} MAC to DE:AD:BE:EF:13:37 took {setMacResult.RunTime.TotalMilliseconds}ms");
                var devUpResult = await CliWrap.Cli.Wrap("ip").WithArguments($"link set {Device.Name} up").WithValidation(CliWrap.CommandResultValidation.None).ExecuteAsync();
                Console.WriteLine($"-- Bringing up {Device.Name} took {devUpResult.RunTime.TotalMilliseconds}ms");
                BeaconMonitor = new BeaconMonitor("beacons.log");
                ProbeMonitor = new ProbeMonitor("probes.log");
                Device.OnPacketArrival += Device_OnPacketArrival;
                Device.Open(DeviceMode.Promiscuous, 2000, MonitorMode.Inactive);
                Device.StartCapture();

                Console.CancelKeyPress += async (a, b) => await MonitoringMode(false);
            }
            else
            {
                Console.WriteLine($"-- Disabling Monitor mode on {Device.Name}");
                Device.StopCapture();
                Console.WriteLine("-- Capture stopped.");
                await CliWrap.Cli.Wrap("ip").WithArguments($"link set {Device.Name} down").WithValidation(CliWrap.CommandResultValidation.None).ExecuteAsync();
                await CliWrap.Cli.Wrap("iwconfig").WithArguments($"{Device.Name} mode Managed").WithValidation(CliWrap.CommandResultValidation.None).ExecuteAsync();
                await CliWrap.Cli.Wrap("ip").WithArguments($"sudo ip link set dev {Device.Name} address 12:7:00:00:00:01").WithValidation(CliWrap.CommandResultValidation.None).ExecuteAsync();
                await CliWrap.Cli.Wrap("ip").WithArguments($"link set {Device.Name} up").WithValidation(CliWrap.CommandResultValidation.None).ExecuteAsync();
            }
        }
    }
}

