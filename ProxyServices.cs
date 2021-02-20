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
     public class Services
     {
         public void ReadFile(IFormFile file)
        {
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while(reader.Peek() >= 0)
                {
                    var endpoint = reader.ReadLine();
                    if(CheckIfEndPoint(endpoint))
                        Proxies.EndPoints.Enqueue(endpoint);
                }
            }
        }
         private bool CheckIfEndPoint(string endpoint)
        {
            int dotcount = endpoint.Length - endpoint.Replace(".","").Length;
            int coloncount = endpoint.Length - endpoint.Replace(":","").Length;
            
            if (dotcount == 3 && coloncount == 1)
                {
                    ushort port;
                    if(ushort.TryParse(endpoint.Split(':')[1], out port))
                    {
                        foreach (var part in endpoint.Split(':')[0].Split('.'))
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
        public bool CheckFileSize(IFormFile file)
        {
            if(file.Length > 0)
                return true;
            else
                return false;
        }
        public string GetEndpoint()
        {
            return Proxies.EndPoints.Dequeue();
        }
     }
 }