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
 
             if(CheckFileSize(file))
             using (var reader = new StreamReader(file.OpenReadStream()))
             {
                 while(reader.Peek() >= 0)
                 {
                     var proxy = reader.ReadLine();
                     if(CheckIfProxy(proxy))
                    {
                        if (RandomNumber(2) == 1)
                        _proxyRepository.AddProxy(new Proxy(proxy,"Germany"));
                        else 
                        _proxyRepository.AddProxy(new Proxy(proxy, "America"));
                    }
                 }
             }
         }
        
        private bool CheckIfProxy(string proxy)
        {
            int dotcount = proxy.Length - proxy.Replace(".","").Length;

            if (dotcount != 3) 
                return false;

            int coloncount = proxy.Length - proxy.Replace(":","").Length;
            
            if (coloncount != 1)
                return false;

           var proxyParts = proxy.Split(':');
           var portString = proxyParts[1];                   
            if(!ushort.TryParse(portString, out _))
                return false;

            var ipParts = proxyParts[0].Split('.');
            foreach (var part in ipParts)
                if(!byte.TryParse(part, out _))
                    return false;
        
            return true;
        }
        private bool CheckFileSize(IFormFile file)
        {
            if(file.Length > 0)
                return true;
            else
                return false;
        }
        public Proxy GetRandomProxy()
        {
            var dblength = _proxyRepository.GetDBLength();
            var random = new System.Random();
            var randomnum = random.Next(dblength);
            return _proxyRepository.GetProxy(randomnum);
        }
        public int RandomNumber(int range)
        {
            var random = new System.Random();
            return random.Next(range); 
        }
        public IQueryable GetProxies(string country, string region, int latency)
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
        public Proxy GetFirstProxy()
        {
            return _proxyRepository.GetFirstProxy();
        }
     }
 }