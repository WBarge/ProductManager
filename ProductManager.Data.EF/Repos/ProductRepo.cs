using System.Linq.Expressions;
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
        IEnumerable<IProduct> results = null!;
        //filterCriteria ??= new Dictionary<string, IFilterMetaData[]>();
        //LambdaExpression filterExpression = FilteringEngine.BuildLambdaExpression<Product>(filterCriteria);
        results = await FindByConditionPagedAsync(filterCriteria, pageNumber, pageSize, cancellationToken);
        return results;
    }

    /// <summary>
    /// Get a count of products as an asynchronous operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;System.Int64&gt; representing the asynchronous operation.</returns>
    public async Task<long> GetProductCountAsync(CancellationToken cancellationToken)
    {
        long result = 0;
        await Task.Run(()=>{
            result = DbContext.Products.LongCount();
            return Task.CompletedTask;
        }, cancellationToken).WaitAsync(cancellationToken);
        return result;
    }
}