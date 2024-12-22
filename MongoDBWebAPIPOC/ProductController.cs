using Microsoft.AspNetCore.Mvc;

namespace MongoDBWebAPIPOC
{
    public class ProductController : ControllerBase
    {
        private readonly productService _service;

        public ProductController(productService service)
        {
            _service = service;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var models = await _service.GetAllAsync();
            return Ok(models);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var model = await _service.GetByIdAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateProductDto model)
        {
            var product = new Product
            {
                Name = model.Name,
                Price = model.Price
            };
            await _service.CreateAsync(product);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Product model)
        {
            await _service.UpdateAsync(id, model);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
