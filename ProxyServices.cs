using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using Microsoft.AspNetCore.Http;
 namespace ProxyAPI
 {
     public class ProxyServices
     {
         public void ReadFile(IFormFile file)
        {
            if(CheckFileSize(file))
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while(reader.Peek() >= 0)
                {
                    var proxy = reader.ReadLine();
                    if(CheckIfProxy(proxy))
                        continue;
                        //ProxyRepository.Proxies.Enqueue(proxy);
                }
            }
        }
         private bool CheckIfProxy(string proxy)
        {
            int dotcount = proxy.Length - proxy.Replace(".","").Length;
            int coloncount = proxy.Length - proxy.Replace(":","").Length;
            
            if (dotcount == 3 && coloncount == 1)
                {
                    ushort port;
                    if(ushort.TryParse(proxy.Split(':')[1], out port))
                    {
                        foreach (var part in proxy.Split(':')[0].Split('.'))
                            {
                                byte bytepart;
                                if(!byte.TryParse(part, out bytepart))
                                    return false;
                            }
                            return true;
                    }    
                    else
                        return false;
                }
            else 
                return false;
        }
        private bool CheckFileSize(IFormFile file)
        {
            if(file.Length > 0)
                return true;
            else
                return false;
        }
     }
 }