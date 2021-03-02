using System.IO;
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
                        _proxyRepository.AddProxy(new Proxy(proxy));
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
            var random = new System.Random();
            var randomnum = random.Next(100);
            return _proxyRepository.GetProxy(randomnum);
        }
     }
 }