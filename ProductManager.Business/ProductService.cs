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
    private readonly IProductRepo _productRepo;

    private readonly IProductOptionRepo _productOptionRepo;
    private readonly IProductCharacteristicRepo _productCharacteristicRepo;


    //******************************************
    // NOTE: THE CONSTRUCTOR IS AT THE MAXIMUM NUMBER OF INJECTED DEPENDENCIES.
    //       IF YOU NEED TO ADD MORE DEPENDENCIES, CONSIDER REFACTORING THE SERVICE INTO MULTIPLE SERVICES.
    //******************************************

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductService" /> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="productRepo">The product repo.</param>
    /// <exception cref="ArgumentNullException">logger</exception>
    /// <exception cref="ArgumentNullException">productRepo</exception>
    public ProductService(ILogger<ProductService> logger, 
        IProductRepo productRepo, 
        IProductOptionRepo productOptionRepo,
        IProductCharacteristicRepo productCharacteristicRepo)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _productRepo = productRepo ?? throw new ArgumentNullException(nameof(productRepo));
        _productOptionRepo = productOptionRepo ?? throw new ArgumentNullException(nameof(productOptionRepo));
        _productCharacteristicRepo = productCharacteristicRepo ?? throw new ArgumentNullException(nameof(productCharacteristicRepo));
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
        _logger.LogDebug("GetProductsAsync called");
        return await _productRepo.FindPagedProductRecordsAsync(filters, page, pageSize, cancellationToken);
    }

    /// <summary>
    /// Get product count as an asynchronous operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;System.Int64&gt; representing the asynchronous operation.</returns>
    public async Task<long> GetProductCountAsync(CancellationToken cancellationToken = default)
    {
        return await _productRepo.GetProductCountAsync(cancellationToken);
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
        IProduct p = _productRepo.CreateInstance();
        p.Name = name;
        p.ShortDescription = shortDescription;
        p.Sku = sku;
        p.Price = price;
        p.Description = "Currently unavailable";
        return await _productRepo.AddMinimumProductAsync(p, cancellationToken);
    }

    /// <summary>
    /// Delete product as an asynchronous operation.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task DeleteProductAsync(Guid id,CancellationToken cancellationToken = default)
    {
        await _productRepo.DeleteAsync(id, cancellationToken);
    }

    /// <summary>
    /// Get product as an asynchronous operation.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;IFullProduct&gt; representing the asynchronous operation.</returns>
    public async Task<IFullProduct?> GetProductAsync(Guid id,CancellationToken cancellationToken = default)
    {
       return await _productRepo.GetProductAsync(id, cancellationToken);
    }

    public async Task<bool> AddProductOptionAsync(Guid productId, Guid optionId, decimal priceOverride, CancellationToken cancellationToken = default)
    {
        if (productId == Guid.Empty || optionId == Guid.Empty)
        {
            throw new ArgumentException("Invalid product or option ID");
        }
        IProductOption productOption = _productOptionRepo.CreateInstance();
        productOption.ProductId = productId;
        productOption.OptionId = optionId;
        productOption.Price = priceOverride > 0 ? priceOverride : decimal.Zero;
        await _productOptionRepo.AddProductOptionAsync(productOption, cancellationToken);
        return true;
    }

    public async Task DeleteProductOptionAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _productOptionRepo.DeleteAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<IProductCharacteristic>> ListProductCharacteristicsAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _productCharacteristicRepo.ListProductCharacteristicsAsync(productId, cancellationToken);
    }

    public async Task<Guid> AddProductCharacteristic(Guid productId, string name, string value,
        CancellationToken cancellationToken = default)
    {
        if (productId == Guid.Empty || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(value))
        {
            throw new ArgumentException("Invalid product or characteristic ID or value");
        }
        IProductCharacteristic productCharacteristic = _productCharacteristicRepo.CreateInstance();
        productCharacteristic.ProductId = productId;
        productCharacteristic.Name = name;
        productCharacteristic.CharacteristicValue = value;

        return await _productCharacteristicRepo.AddProductCharacteristicAsync(productCharacteristic, cancellationToken);

    }

    public async Task DeleteProductCharacteristicAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _productCharacteristicRepo.DeleteAsync(id, cancellationToken);
    }

    public async Task UpdateProductAsync(IFullProduct product, CancellationToken cancellationToken = default)
    {
        if (product == null)
        {
            throw new ArgumentNullException(nameof(product));
        }

        if (product.Sells.Any())
        {
            List<IProductSell> originalList = product.Sells.ToList();
            List<IProductSell> toRemove = new List<IProductSell>();
            //leave as old nested for-each for maintainability
            foreach (IProductSell productSell in originalList)
            {
                foreach (IProductSell otherSell in originalList)
                {
                    if (otherSell == productSell)
                    {
                        continue;
                    }

                    if (
                        (otherSell.Period.WithIn(productSell.Period) || otherSell.Period.Overlaps(productSell.Period)) &&
                        !(toRemove.Contains(otherSell) || toRemove.Contains(productSell))
                        )
                    {
                        toRemove.Add(otherSell);
                    }
                }
            }

            foreach (IProductSell productSell in toRemove)
            {
                originalList.Remove(productSell);    
            }

            if (originalList.Count != product.Sells.Count())
            {
                product.Sells = originalList;
            }

            await _productRepo.UpdateProductAsync(product,cancellationToken);
        }
    }
}   