// ***********************************************************************
// Author           : Bill Barge
// Created          : 08-16-2024
//
// Last Modified By : Bill Barge
// Last Modified On : 08-20-2024
// ***********************************************************************
// <copyright file="ProductService.cs" company="N/A">
//     Copyright (c) N/A. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.Extensions.Logging;
using ProductManager.Glue.Interfaces.Models;
using ProductManager.Glue.Interfaces.Repos;
using ProductManager.Glue.Interfaces.Services;

namespace ProductManager.Business;

/// <summary>
/// Class ProductService.
/// Implements the <see cref="IProductService" />
/// </summary>
/// <seealso cref="IProductService" />
public class ProductService : IProductService
{
    /// <summary>
    /// The logger
    /// </summary>
    private readonly ILogger<ProductService> _logger;
    /// <summary>
    /// The repo
    /// </summary>
    private readonly IProductRepo _repo;


    /// <summary>
    /// Initializes a new instance of the <see cref="ProductService" /> class.
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

    /// <summary>
    /// Get product count as an asynchronous operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;System.Int64&gt; representing the asynchronous operation.</returns>
    public async Task<long> GetProductCountAsync(CancellationToken cancellationToken = default)
    {
        return await _repo.GetProductCountAsync(cancellationToken);
    }

    /// <summary>
    /// Create minimum viable product as an asynchronous operation.
    /// </summary>
    /// <param name="sku">The sku.</param>
    /// <param name="name">The name.</param>
    /// <param name="shortDescription">The short description.</param>
    /// <param name="price">The price.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task<Guid> CreateMinimumViableProductAsync(string sku, string name, string shortDescription, decimal price,
        CancellationToken cancellationToken = default)
    {
        IProduct p = _repo.CreateInstance();
        p.Name = name;
        p.ShortDescription = shortDescription;
        p.Sku = sku;
        p.Price = price;
        p.Description = "Currently unavailable";
        return await _repo.AddMinimumProductAsync(p, cancellationToken);
    }

    /// <summary>
    /// Delete product as an asynchronous operation.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task DeleteProductAsync(Guid id,CancellationToken cancellationToken = default)
    {
        await _repo.DeleteAsync(id, cancellationToken);
    }
}