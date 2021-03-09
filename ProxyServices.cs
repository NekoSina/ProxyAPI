using System.IO;
using System.Linq;
using ProxyAPI.Models;
using Microsoft.AspNetCore.Http;
namespace ProxyAPI
{
    public class ProxyServices
    {
        private ProxyRepository _proxyRepository;
        public ProxyServices(ProxyRepository repository)
        {
            _proxyRepository = repository;
        }
        public void ReadFile(IFormFile file)
        {
            if (!CheckFileSize(file))
                return;

            using var reader = new StreamReader(file.OpenReadStream());

            while(!reader.EndOfStream)
            {
                var proxy = reader.ReadLine();
                if (CheckIfProxy(proxy))
                {
                    //THIS NEEDS WORK
                    if (Random.RandomNumber(2) == 1)
                        _proxyRepository.AddProxy(new Proxy(proxy, "Germany"));
                    else
                        _proxyRepository.AddProxy(new Proxy(proxy, "America"));
                }
            }
        }

        private bool CheckIfProxy(string proxy)
        {
            int dotcount = proxy.Length - proxy.Replace(".", "").Length;

            if (dotcount != 3)
                return false;

            int coloncount = proxy.Length - proxy.Replace(":", "").Length;

            if (coloncount != 1)
                return false;

            var proxyParts = proxy.Split(':');
            var portString = proxyParts[1];
            if (!ushort.TryParse(portString, out _))
                return false;

            var ipParts = proxyParts[0].Split('.');
            foreach (var part in ipParts)
                if (!byte.TryParse(part, out _))
                    return false;

            return true;
        }

        internal void AddProxy(Proxy proxy)
        {
            //add proxy (change proxy.Id to be unique)
            //save changes
            _proxyRepository.AddProxy(proxy);
        }

        internal void UpdateProxy(Proxy proxy)
        {
            //find proxy in db  
            //update proxy
            //save changes
            _proxyRepository.UpdateProxy(proxy);
        }

        private bool CheckFileSize(IFormFile file)
        {
            var size = 1024*1024;
            if(file.Length < size)
                return true;
            else
                return false;
        }
        public Proxy GetRandomProxy()
        {
            var dblength = _proxyRepository.GetDBLength();
            var randomnum = Random.RandomNumber(dblength);
            return _proxyRepository.GetProxy(randomnum);
        }
        public IQueryable GetProxies(string country, string region)
        {
            return _proxyRepository.GetProxies(country, region);
        }
        public void Cleardb()
        {
            _proxyRepository.Cleardb();
        }
        public int GetDBLength()
        {
            return _proxyRepository.GetDBLength();
        }
    }
}