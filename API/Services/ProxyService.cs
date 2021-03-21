using System.IO;
using System.Linq;
using ProxyAPI.Models;
using Microsoft.AspNetCore.Http;
using ProxyAPI.Repositories;
using ProxyAPI.Logic;
using System.Collections.Generic;

namespace ProxyAPI.Services
{
    public class ProxyService
    {
        private ProxyRepository _proxyRepository;
        public ProxyService(ProxyRepository repository) => _proxyRepository = repository;

        public void ReadFile(IFormFile file)
        {
            if (file == null)
                return;
            if (!CheckFileSize(file))
                return;

            using var reader = new StreamReader(file.OpenReadStream());

            while (!reader.EndOfStream)
                ProxyListCruncher.Enqueue(reader.ReadLine());
        }

        internal void AddProxy(Proxy proxy) => _proxyRepository.AddOrUpdateProxy(proxy);
        internal void UpdateProxy(Proxy proxy) => _proxyRepository.AddOrUpdateProxy(proxy);

        private bool CheckFileSize(IFormFile file)
        {
            var size = 10 * 1024 * 1024;
            if (file.Length < size)
                return true;
            else
                return false;
        }
        public IEnumerable<Proxy> GetProxies(string country, string region,int hoursSinceTest) 
        {
            return _proxyRepository.GetProxies(country, region,hoursSinceTest);
            // var count = proxies.Count();

            // if(count ==0)
            //     return null;

            // var idx = Helpers.Random.Next(0,count);
            // return proxies.Skip(idx).First();
        }
        public void DeleteProxy(uint id)
        {
            _proxyRepository.DeleteProxy(id);
        }
    }
}