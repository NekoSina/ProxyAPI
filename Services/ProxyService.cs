using System.IO;
using ProxyAPI.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using ProxyAPI.Repositories;
using ProxyAPI.Logic;

namespace ProxyAPI.Services
{
    public class ProxyService
    {
        private ProxyRepository _proxyRepository;
        public ProxyService(ProxyRepository repository)
        {
            _proxyRepository = repository;
        }
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
            if (file.Length < size)
                return true;
            else
                return false;
        }
        public Proxy GetRandomProxy(string country, string region)
        {
            var tempList = new List<Proxy>();
            foreach (Proxy proxy in _proxyRepository.GetProxies(country, region))
                tempList.Add(proxy);

            if (tempList.Count == 0)
                return null;

            var randomNum = Helpers.Random.Next(tempList.Count);
            return tempList[randomNum];
        }
        public void DeleteProxy(int id)
        {
            _proxyRepository.DeleteProxy(id);
            _proxyRepository.Save();
        }
    }
}