using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcWebApp.Models;
using MvcWebApp.Repositories;

namespace MvcWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductApiController : ControllerBase
    {
        private readonly IRepository<Product> repository;

        public ProductApiController(IRepository<Product> repository)
        {
            this.repository = repository;
        }

        // GET: api/ProductApi
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            return Ok(await repository.GetAll());
        }

        // GET: api/ProductApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetProduct(int id)
        {
            var product = await repository.GetById(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // PUT: api/ProductApi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public IActionResult PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            repository.Update(product);

            return NoContent();
        }

        // POST: api/ProductApi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostProduct(Product product)
        {
            await repository.Create(product);

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        // DELETE: api/ProductApi/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var product = await repository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            repository.Delete(product);

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return repository.GetById(id).Result == null ? false : true;
        }
    }
}
