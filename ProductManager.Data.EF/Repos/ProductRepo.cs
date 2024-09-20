using CrossCutting.Extensions;
using Microsoft.EntityFrameworkCore;
using ProductManager.Data.EF.Helpers;
using ProductManager.Data.EF.Model;
using ProductManager.Glue.Interfaces.Models;
using ProductManager.Glue.Interfaces.Repos;

namespace ProductManager.Data.EF.Repos;

/// <summary>
/// Class ProductRepo.
/// </summary>
/// <seealso cref="BaseEfRepo{Product}" />
public class ProductRepo : BaseEfRepo<Product>, IProductRepo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProductRepo"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    public ProductRepo(ProductDbContext dbContext) : base(dbContext)
    {
    }

    /// <summary>
    /// Gets the records that matches the filter criteria
    /// We will not get any records that are marked as deleted 
    /// </summary>
    /// <param name="filterCriteria"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IEnumerable<IProduct>> FindPagedProductRecordsAsync(
        Dictionary<string, IFilterMetaData[]> filterCriteria = null!,
        int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        filterCriteria ??= new Dictionary<string, IFilterMetaData[]>();
        IFilterMetaData[] list = new IFilterMetaData[1];
        list[0] = new FilterCriteria(){SearchValue = "False",
            MatchMode = FilteringEngine.EQUALS_COMPARISON,
            LogicalOperator = FilteringEngine.AND_LOGICAL_OPERATOR};
        filterCriteria.Add("Deleted", list);
        IEnumerable<IProduct> results =
            await FindByConditionPagedAsync(filterCriteria, pageNumber, pageSize, cancellationToken);
        return results;
    }

    /// <summary>
    /// Get a count of products as an asynchronous operation.
    /// We will not count any records that are marked as deleted
    /// </summary>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;System.Int64&gt; representing the asynchronous operation.</returns>
    public async Task<long> GetProductCountAsync(CancellationToken cancellationToken= default)
    {
        long result = 0;
        await Task.Run(()=>{
            result = DbContext.Products.Where(p=>p.Deleted==false).LongCount();
            return Task.CompletedTask;
        }, cancellationToken).WaitAsync(cancellationToken);
        return result;
    }

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <returns>IProduct.</returns>
    public IProduct CreateInstance()
    {
        return Create();
    }

    /// <summary>
    /// Add minimum product as an asynchronous operation.
    /// </summary>
    /// <param name="product">The product.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task<Guid> AddMinimumProductAsync(IProduct product,CancellationToken cancellationToken = default)
    {
        Product p = Create();
        p.Name = product.Name;
        p.Sku = product.Sku;
        p.ShortDescription = product.ShortDescription;
        p.Price = product.Price;
        p.Description = product.Description;
        await InsertAsync(p);
        await SaveAsync(cancellationToken);
        return p.Id;
    }

    /// <summary>
    /// Delete as an asynchronous operation.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Product p = await this.DbContext.Products.FirstAsync(p => p.Id == id, cancellationToken: cancellationToken);
        if (p.IsNotEmpty())
        {
            p.Deleted = true;
            Update(p);
            await SaveAsync(cancellationToken);
        }
    }
}