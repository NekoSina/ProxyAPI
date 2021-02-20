using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;


namespace ProxyAPI.Controllers
{
    public class UploadFileController : ControllerBase 
    {
        private Services services;
        public UploadFileController()
        {
            services = new Services();
        }
        
        [HttpPost]
        [Route("ProxyAPI/UploadFile/FileUpload")]
        public IActionResult FileUpload(IFormFile file)
        {
            //Checking the File size
            if (services.CheckFileSize(file))
            {
                services.ReadFile(file);
                return Ok();
            }
            else return BadRequest();
        }
        [HttpGet]
        [Route("ProxyAPI/UploadFile/GetEndPoint")]
        public string GetEndPoint()
        {
            return services.GetEndpoint();
        }
    }
}