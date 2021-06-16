using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using libherst;
using libwifimonitor;
using libwifimonitor.PacketMonitors;
using PacketDotNet.Ieee80211;
using SharpPcap;

namespace HerstReconWiFiMonitor
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<CacheEntry> Probes { get; set; }
        public ObservableCollection<CacheEntry> Beacons { get; set; }

        internal static BeaconMonitor BeaconMonitor;
        internal static ProbeMonitor ProbeMonitor;
        internal static ICaptureDevice Device;

        public MainWindow()
        {
            Probes = new ObservableCollection<CacheEntry>();
            Beacons = new ObservableCollection<CacheEntry>();
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            DataContext = this;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async void OnOpened(object sender, EventArgs _)
        {
            Device = FindDevice();
            try
            {
                await MonitoringMode(true);
                Console.WriteLine($"-- Listening on {Device.Name}, hit 'Enter' to stop...");

                Task.Run(() =>
                {
                    while (true)
                    {
                        Probes.Clear();
                        foreach (var kvp in ProbeCache.Cache)
                        foreach (var ce in kvp.Value)
                            Probes.Add(ce);
                        Beacons.Clear();
                        foreach (var kvp in BeaconCache.Cache)
                        foreach (var ce in kvp.Value)
                            Beacons.Add(ce);
                        Thread.Sleep(1000);
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        private static ICaptureDevice FindDevice() => CaptureDeviceList.Instance.FirstOrDefault(dev => dev.Name.Contains("wlan") || dev.Name.Contains("wlp"));

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
                var wifiOffResult = await CliWrap.Cli.Wrap("sudo").WithArguments($"nmcli radio wifi off").WithValidation(CliWrap.CommandResultValidation.None).ExecuteAsync();
                Console.WriteLine($"-- Disabling the WiFi chip took {wifiOffResult.RunTime.TotalMilliseconds}ms");
                var wifiOnResult = await CliWrap.Cli.Wrap("sudo").WithArguments($"nmcli radio wifi on").WithValidation(CliWrap.CommandResultValidation.None).ExecuteAsync();
                Console.WriteLine($"-- Enabling the WiFi chip took {wifiOnResult.RunTime.TotalMilliseconds}ms");
                var wwanOnResult = await CliWrap.Cli.Wrap("sudo").WithArguments($"nmcli radio wwan on").WithValidation(CliWrap.CommandResultValidation.None).ExecuteAsync();
                Console.WriteLine($"-- Enabling the modem took {wwanOnResult.RunTime.TotalMilliseconds}ms");
                var lteUpResult = await CliWrap.Cli.Wrap("sudo").WithArguments($"nmcli connection PublicIP up").WithValidation(CliWrap.CommandResultValidation.None).ExecuteAsync();
                Console.WriteLine($"-- Connecting to 4G took {lteUpResult.RunTime.TotalMilliseconds}ms");
                var devDownResult = await CliWrap.Cli.Wrap("sudo").WithArguments($"ip link set {Device.Name} down").WithValidation(CliWrap.CommandResultValidation.None).ExecuteAsync();
                Console.WriteLine($"-- Taking down {Device.Name} took {devDownResult.RunTime.TotalMilliseconds}ms");
                var modeResult = await CliWrap.Cli.Wrap("sudo").WithArguments($"iwconfig {Device.Name} mode Monitor").WithValidation(CliWrap.CommandResultValidation.None).ExecuteAsync();
                Console.WriteLine($"-- Switching {Device.Name} to Monitor Mode took {modeResult.RunTime.TotalMilliseconds}ms");
                var setMacResult = await CliWrap.Cli.Wrap("sudo").WithArguments($"ip link set dev {Device.Name} address DE:AD:BE:EF:13:37").WithValidation(CliWrap.CommandResultValidation.None).ExecuteAsync();
                Console.WriteLine($"-- Setting {Device.Name} MAC to DE:AD:BE:EF:13:37 took {setMacResult.RunTime.TotalMilliseconds}ms");
                var devUpResult = await CliWrap.Cli.Wrap("sudo").WithArguments($"ip link set {Device.Name} up").WithValidation(CliWrap.CommandResultValidation.None).ExecuteAsync();
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
                await CliWrap.Cli.Wrap("sudo").WithArguments($"ip link set {Device.Name} down").WithValidation(CliWrap.CommandResultValidation.None).ExecuteAsync();
                await CliWrap.Cli.Wrap("sudo").WithArguments($"iwconfig {Device.Name} mode Managed").WithValidation(CliWrap.CommandResultValidation.None).ExecuteAsync();
                await CliWrap.Cli.Wrap("sudo").WithArguments($"ip link set dev {Device.Name} address 12:7:00:00:00:01").WithValidation(CliWrap.CommandResultValidation.None).ExecuteAsync();
                await CliWrap.Cli.Wrap("sudo").WithArguments($"ip link set {Device.Name} up").WithValidation(CliWrap.CommandResultValidation.None).ExecuteAsync();
            }
        }
        public static string Color(int color) => $"\u001b[38;5;{color}m";
    }
}