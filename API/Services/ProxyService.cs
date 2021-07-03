using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HerstAPI.Logic;
using HerstAPI.Repositories;
using libherst.Models;
using Microsoft.AspNetCore.Http;
using HerstAPI.Cache;

namespace HerstAPI.Services
{
    public class ProxyService
    {
        private readonly ProxyRepository _proxyRepository;
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
        internal void UpdateProxy(Proxy proxy)
        {
            ProxyTestCache.Remove(proxy.Id);
            _proxyRepository.AddOrUpdateProxy(proxy);
        }

        internal IEnumerable<Proxy> GetProxiesToTest(int count)
        {
            var proxies = _proxyRepository.GetProxies(string.Empty, string.Empty).OrderBy(p => p.LastTest).AsEnumerable().Where(p => !ProxyTestCache.Contains(p.Id)).Take(count);
            foreach (var proxy in proxies)
                ProxyTestCache.Add(proxy);
            return proxies;
        }

        private static bool CheckFileSize(IFormFile file)
        {
            var size = 10 * 1024 * 1024;
            if (file.Length < size)
                return true;
            else
                return false;
        }
        public IEnumerable<Proxy> GetProxies(bool working, string country, string region, int hoursSinceTest, int score)
        {
            return _proxyRepository.GetProxies(country, region)
                       .Where(p => hoursSinceTest == 0 || p.LastTest.AddHours(hoursSinceTest) > DateTime.UtcNow)
                       .Where(p => score == 0 || p.Score == score)
                       .Where(p=> working == false || p.Working == working);
        }
        public void DeleteProxy(uint id)
        {
            _proxyRepository.DeleteProxy(id);
        }
    }
}