using Microsoft.Extensions.Logging;
using ProductManager.Glue.Interfaces.Models;
using ProductManager.Glue.Interfaces.Repos;
using ProductManager.Glue.Interfaces.Services;

namespace ProductManager.Business;

public class ProductService : IProductService
{
    private readonly ILogger<ProductService> _logger;
    private readonly IProductRepo _repo;


    /// <summary>
    /// Initializes a new instance of the <see cref="ProductService"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="productRepo">The product repo.</param>
    /// <exception cref="ArgumentNullException">logger</exception>
    /// <exception cref="ArgumentNullException">productRepo</exception>
    public ProductService(ILogger<ProductService> logger, IProductRepo productRepo)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _repo = productRepo ?? throw new ArgumentNullException(nameof(productRepo));
    }

    /// <summary>
    /// Get short products as an asynchronous operation.
    /// </summary>
    /// <param name="filters">The filters.</param>
    /// <param name="page">The page.</param>
    /// <param name="pageSize">Size of the page.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;IEnumerable`1&gt; representing the asynchronous operation.</returns>
    public async Task<IEnumerable<IProduct>> GetProductsAsync(Dictionary<string, IFilterMetaData[]> filters,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("GetShortProductsAsync called");
        return await _repo.FindPagedProductRecordsAsync(filters, page, pageSize, cancellationToken);
    }

    public async Task<long> GetProductCountAsync(CancellationToken cancellationToken = default)
    {
        return await _repo.GetProductCountAsync(cancellationToken);
    }
}