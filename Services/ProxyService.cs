using System.IO;
using ProxyAPI.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using ProxyAPI.Repositories;
using ProxyAPI.IP2Location;
using System.Net;
using System;

namespace ProxyAPI.Services
{
    public class ProxyServices
    {
        private static IpLocator Locator = new BinaryDbClient("ipdb.bin");
        private ProxyRepository _proxyRepository;
        public ProxyServices(ProxyRepository repository)
        {
            _proxyRepository = repository;
        }
        public void ReadFile(IFormFile file)
        {
            if(file == null)
                return;
            if (!CheckFileSize(file))
                return;

            try
            {
            using var reader = new StreamReader(file.OpenReadStream());

            while(!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                Console.WriteLine(line);
                if (CheckIfProxy(line, out var ip, out var port))
                {
                    var location = Locator.Locate(IPAddress.Parse(ip));
                    var proxy = new Proxy 
                    { 
                        ID = Helpers.IpToInt(ip),
                        IP=ip,
                        Port=port,
                        City = location.City,
                        Country = location.Country,
                        Region = location.Region
                    };

                    _proxyRepository.AddOrUpdateProxy(proxy);
                }
            }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine("Finished parsing uploaded list.");
            _proxyRepository.Save();
            Console.WriteLine("Saving....");
        }

        private bool CheckIfProxy(string proxy, out string ip, out ushort port)
        {
            ip = "127.0.0.1";
            port = 0;

            int dotcount = proxy.Length - proxy.Replace(".", "").Length;

            if (dotcount != 3)
                return false;

            int coloncount = proxy.Length - proxy.Replace(":", "").Length;

            if (coloncount != 1)
                return false;

            var proxyParts = proxy.Split(':');
            var portString = proxyParts[1];

            if (!ushort.TryParse(portString, out port))
                return false;

            var ipParts = proxyParts[0].Split('.');
            
            foreach (var part in ipParts)
                if (!byte.TryParse(part, out _))
                    return false;
            
            ip = proxyParts[0];

            return true;
        }

        internal void AddProxy(Proxy proxy)
        {
            _proxyRepository.AddOrUpdateProxy(proxy);
            _proxyRepository.Save();
        }

        internal void UpdateProxy(Proxy proxy)
        {
            _proxyRepository.AddOrUpdateProxy(proxy);
            _proxyRepository.Save();
        }

        private bool CheckFileSize(IFormFile file)
        {
            var size = 10 * 1024 * 1024;
            if(file.Length < size)
                return true;
            else
                return false;
        }
        public Proxy GetRandomProxy(string country, string region)
        {
            var tempList = new List<Proxy>();
            foreach(Proxy proxy in _proxyRepository.GetProxies(country,region))
                tempList.Add(proxy);
            
            if(tempList.Count == 0)
                return null;

            var randomNum = Helpers.Random.Next(tempList.Count);
            return tempList[randomNum];            
        }
        public void Cleardb()
        {
            _proxyRepository.Cleardb();
        }
        public void DeleteProxy(int id)
        {
            _proxyRepository.DeleteProxy(id);
            _proxyRepository.Save();
        }
        public int GetDBLength()
        {
            return _proxyRepository.GetDBLength();
        }
    }
}