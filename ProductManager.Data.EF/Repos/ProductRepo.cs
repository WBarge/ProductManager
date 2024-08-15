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
    public async Task<IEnumerable<IShortProduct>> FindPagedShortProductRecordsAsync(
        Dictionary<string, IFilterMetaData[]> filterCriteria = null!,
        int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        IEnumerable<IShortProduct> results = null!;
        //filterCriteria ??= new Dictionary<string, IFilterMetaData[]>();
        //LambdaExpression filterExpression = FilteringEngine.BuildLambdaExpression<Product>(filterCriteria);
        results = await FindByConditionPagedAsync(filterCriteria, pageNumber, pageSize, cancellationToken);
        return results;
    }
         

}