using System;
using System.Collections.Generic;
using System.Threading;
using libherst;
using libherst.Models;

namespace libherst
{
    public static class ApiQueue
    {
        private static readonly TrblWiFiApiClient TrblClient;
        private static readonly Queue<WiFiProbe> ProbeQUeue = new();
        private static readonly Queue<WiFiAccessPoint> AccessPointQUeue = new();
        private static readonly Thread BeaconSubmissionThread;
        private static readonly Thread ProbeSubmissionThread;

        private static readonly AutoResetEvent BeaconBlock = new(true);
        private static readonly AutoResetEvent ProbeBlock = new(true);

        static ApiQueue()
        {
            TrblClient = new TrblWiFiApiClient();
            TrblClient.LoginAsync(null, null).GetAwaiter().GetResult();
            Console.WriteLine("-- Connected to Her.st Intelligence API!");

            BeaconSubmissionThread = new Thread(BeaconSubmissionWorkLoop) {IsBackground = true};
            BeaconSubmissionThread.Start();

            ProbeSubmissionThread = new Thread(ProbeSubmissionWorkLoop) {IsBackground = true};
            ProbeSubmissionThread.Start();
        }

        private static async void BeaconSubmissionWorkLoop(object obj)
        {
            while (true)
            {
                BeaconBlock.WaitOne();
                while (AccessPointQUeue.Count > 0)
                {
                    var ap = AccessPointQUeue.Dequeue();
                    if (await TrblClient.SubmitBeacon(ap)) 
                        continue;
                    
                    Console.WriteLine("Failed to submit " + ap.WiFiNetworkName);
                    AccessPointQUeue.Enqueue(ap);
                }
            }
        }
        private static async void ProbeSubmissionWorkLoop(object obj)
        {
            while (true)
            {
                ProbeBlock.WaitOne();
                while (ProbeQUeue.Count > 0)
                {
                    var probe = ProbeQUeue.Dequeue();
                    if (await TrblClient.SubmitProbe(probe)) 
                        continue;
                    
                    Console.WriteLine("Failed to submit " + probe.WiFiNetworkName);
                    ProbeQUeue.Enqueue(probe);
                }
            }
        }

        public static void QueueItem(WiFiProbe probe)
        {
            ProbeQUeue.Enqueue(probe);
            ProbeBlock.Set();
        }
        public static void QueueItem(WiFiAccessPoint accessPoint)
        {
            AccessPointQUeue.Enqueue(accessPoint);
            BeaconBlock.Set();
        }
    }
}