using Microsoft.AspNetCore.Mvc;
using ProductManager.Glue.Interfaces.Models;
using ProductManager.Glue.Interfaces.Services;
using ProductManager.Service.Models.Request;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProductManager.Service.Controllers
{
    /// <summary>
    /// Controller for crud operations on a single product
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<ProductController> _logger;

        /// <summary>
        /// The product service
        /// </summary>
        private readonly IProductService _productService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductsController" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="productService">The product service.</param>
        /// <exception cref="ArgumentNullException">logger</exception>
        /// <exception cref="ArgumentNullException">productService</exception>
        public ProductController(ILogger<ProductController> logger, IProductService productService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        /// <summary>
        /// Gets the product with full details.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The product</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(Guid id)
        {
            _logger.LogDebug("Get Product received request");
            IFullProduct? product = await _productService.GetProductAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return new OkObjectResult(product);
        }

        /// <summary>
        /// end point which adds the minimum viable product
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        [Route(nameof(QuickAdd))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async  Task<IActionResult> QuickAdd([FromBody] QuickProductRequest request)
        {
            _logger.LogDebug("request to quick product add");
            Guid newId =  await _productService.CreateMinimumViableProductAsync(request.Sku, request.Name, request.ShortDescription, request.Price);
            return new OkObjectResult(newId);
        }

        
        // POST api/<ProductController>
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        /// <summary>
        /// Deletes the product.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>IActionResult.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async  Task<IActionResult> DeleteProduct(Guid id)
        {
            _logger.LogDebug("request to delete product");
            await this._productService.DeleteProductAsync(id);
            return Ok();
        }
    }
}
