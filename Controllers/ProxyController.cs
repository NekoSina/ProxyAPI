using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProxyAPI.Models;

namespace ProxyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProxyController : ControllerBase
    {
        private readonly ProxyContext _context;
        private ProxyServices _services;
        public ProxyController(ProxyContext context)
        {
            _services = new ProxyServices();
            _context = context;
        }

        // GET: api/Proxy
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Proxy>>> GetProxies()
        {
            return await _context.Proxies.ToListAsync();
        }

        // GET: api/Proxy/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Proxy>> GetProxy(int id)
        {
            var proxy = await _context.Proxies.FindAsync(id);

            if (proxy == null)
            {
                return NotFound();
            }

            return proxy;
        }

        // PUT: api/Proxy/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProxy(int id, Proxy proxy)
        {
            if (id != proxy.ID)
            {
                return BadRequest();
            }

            _context.Entry(proxy).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProxyExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Proxy
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Proxy>> PostProxy(Proxy proxy)
        {
            _context.Proxies.Add(proxy);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProxy", new { id = proxy.ID }, proxy);
        }
        [HttpPost]
        [Route("/api/UploadFile")]
        public IActionResult UploadFile(IFormFile file)
        {
        
            return Ok();
        }
        // DELETE: api/Proxy/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProxy(int id)
        {
            var proxy = await _context.Proxies.FindAsync(id);
            if (proxy == null)
            {
                return NotFound();
            }

            _context.Proxies.Remove(proxy);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProxyExists(int id)
        {
            return _context.Proxies.Any(e => e.ID == id);
        }
    }
}
