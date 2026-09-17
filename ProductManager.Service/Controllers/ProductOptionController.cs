using Microsoft.AspNetCore.Mvc;
using ProductManager.Glue.Interfaces.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProductManager.Service.Controllers
{
    /// <summary>
    /// Represents a controller for managing product options.
    /// </summary>
    [Route("api/Product/{productId:guid}/Option")]
    [ApiController]
    public class ProductOptionController : ControllerBase
    {

        private readonly ILogger<ProductOptionController> _logger;
        private readonly IProductService _productService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductOptionController"/> class.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="productService"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ProductOptionController(ILogger<ProductOptionController> logger, IProductService productService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        // POST api/Product/{productId}/Option/{id}
        /// <summary>
        /// Creates a new product option.
        /// </summary>
        /// <param name="productId">from url</param>
        /// <param name="id">from url</param>
        /// <param name="priceOverride">from body - the number by itself</param>
        [HttpPost("{id:guid}")]
        public async Task<IActionResult> Post(Guid productId,Guid id,[FromBody] decimal priceOverride)
        {
            _logger.LogDebug($"Creating product option for product {productId} with Option ID {id} and a price override of {priceOverride}");
            bool result = await _productService.AddProductOptionAsync(productId, id, priceOverride);
            if (result)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        // DELETE api/<ProductOptionController>/5
        /// <summary>
        /// Deletes a product option.
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="NotImplementedException"></exception>
        [HttpDelete("{id:guid}")]       
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogDebug($"Deleting product option with ID {id}");
            await _productService.DeleteProductOptionAsync(id);
            return Ok();
        }
    }
}
