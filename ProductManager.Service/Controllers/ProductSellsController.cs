using Microsoft.AspNetCore.Mvc;
using ProductManager.Glue.Interfaces.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProductManager.Service.Controllers
{
    /// <summary>
    /// Provides API endpoints for managing sell periods of a specific product.
    /// </summary>
    /// <remarks>
    /// This controller allows clients to add and delete sell periods for a product.
    /// It is designed to handle requests related to the sell periods of a product identified by its unique identifier.
    /// </remarks>
    /// <example>
    /// Example usage:
    /// POST: api/Product/{productId}/Sells
    /// DELETE: api/Product/{productId}/Sells/{id}
    /// </example>
    [Route("api/Product/{productId:guid}/Sells")]
    [ApiController]
    public class ProductSellsController : ControllerBase
    {
        private readonly ILogger<ProductSellsController> _logger;
        private readonly IProductService _productService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductSellsController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance used for logging information and errors.</param>
        /// <param name="productService">The service used to manage product-related operations, including sell periods.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="logger"/> or <paramref name="productService"/> is <c>null</c>.
        /// </exception>
        public ProductSellsController(ILogger<ProductSellsController> logger, IProductService productService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        /// <summary>
        /// Adds a new sell period for the specified product.
        /// </summary>
        /// <param name="productId">The unique identifier of the product for which the sell period is being added.</param>
        /// <param name="start">The start date and time of the sell period.</param>
        /// <param name="end">The end date and time of the sell period.</param>
        /// <param name="price">The price of the product during the sell period.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the unique identifier of the newly created sell period.
        /// </returns>
        /// <remarks>
        /// This method creates a new sell period for a product identified by <paramref name="productId"/>.
        /// The sell period is defined by the <paramref name="start"/> and <paramref name="end"/> parameters.
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// Thrown if the <paramref name="start"/> date is greater than or equal to the <paramref name="end"/> date.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the product with the specified <paramref name="productId"/> does not exist.
        /// </exception>
        /// <response code="200">The sell period was successfully created, and the unique identifier is returned.</response>
        /// <response code="400">The request is invalid, such as when the dates are not valid or the product does not exist.</response>
        /// <response code="500">An internal server error occurred while processing the request.</response>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Guid productId, DateTime start, DateTime end, decimal price)
        {
            CancellationToken token = HttpContext?.RequestAborted ?? CancellationToken.None;

            Guid idOfNewRecord = await _productService.AddSellPeriodAsync(productId, start, end, price,token);
            return new OkObjectResult(idOfNewRecord);
        }


        /// <summary>
        /// Deletes a sell period associated with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the sell period to delete.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.</returns>
        /// <remarks>
        /// This method removes a sell period from the system. If the specified identifier does not exist,
        /// the operation will have no effect.
        /// </remarks>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            CancellationToken token = HttpContext?.RequestAborted ?? CancellationToken.None;

            await _productService.DeleteSellPeriodAsync(id, token);
            return Ok();
        }
    }
}
