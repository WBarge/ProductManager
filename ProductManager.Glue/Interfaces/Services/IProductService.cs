﻿using ProductManager.Glue.Interfaces.Models;

namespace ProductManager.Glue.Interfaces.Services;

public interface IProductService
{
    /// <summary>
    /// Gets the short products asynchronous.
    /// </summary>
    /// <param name="filters">The filters.</param>
    /// <param name="page">The page.</param>
    /// <param name="pageSize">Size of the page.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;IEnumerable&lt;IShortProduct&gt;&gt;.</returns>
    Task<IEnumerable<IProduct>> GetProductsAsync(Dictionary<string, IFilterMetaData[]> filters,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<long> GetProductCountAsync(CancellationToken  cancellationToken = default);
    Task<Guid> CreateMinimumViableProductAsync(string sku, string name, string shortDescription, decimal price,CancellationToken cancellationToken = default);
    Task DeleteProductAsync(Guid id,CancellationToken cancellationToken = default);
}