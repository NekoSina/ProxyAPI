using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using PacketDotNet.Ieee80211;

namespace libwifimonitor
{
    public class BaseMonitor<T>
    {
        public string LogFilePath;
        private readonly StreamWriter _writer;

        protected BaseMonitor(string path) => _writer = new StreamWriter(path, true, Encoding.UTF8) {AutoFlush = true};
        public virtual Task HandlePacket(RadioPacket rp, T frame) => Task.CompletedTask;

        protected void Log(string data)
        {
            _writer.WriteLine(data);
            Console.WriteLine(data);
        }
    }
}