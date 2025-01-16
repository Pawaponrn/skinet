using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IProductRepository repo) : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(string? brand, string? type, string? sort)
        {
            // var products = await context.Products.ToListAsync();
            return Ok(await repo.GetProductsAsync(brand, type, sort));
        }

        [HttpGet("{id:int}")] // api/products/3
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
        var product = await repo.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }
            return product;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            repo.AddProduct(product);
            if (await repo.SaveChangesAsync()) 
            {
                return CreatedAtAction("GetProduct", new { id = product.Id }, product);
            }
            // return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            return BadRequest("Problem creating product");
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateProduct(int id, Product product)
        {
            if (product.Id != id || !ProductExists(id))
            {
                BadRequest("Cannot update product");
            }

            repo.UpdateProduct(product);

            if (await repo.SaveChangesAsync()) 
            {
                return NoContent();
            }

            return BadRequest("Problem updating product");
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await repo.GetProductByIdAsync(id);

            if (product == null) return NotFound();

            repo.DeleteProduct(product);

            if (await repo.SaveChangesAsync()) 
            {
                return NoContent();
            }

            return BadRequest("Problem deleting product");
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
        {
            return Ok(await repo.GetBrandAsync());
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
        {
            return Ok(await repo.GetTypeAsync());
        }

        private bool ProductExists(int id)
        {
            return repo.ProductExists(id);
        }
    }
}
