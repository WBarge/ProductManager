using ProductManager.Glue.Interfaces.Models;

namespace ProductManager.Glue.Interfaces.Repos;

public interface IProductRepo
{
    /// <summary>
    /// Gets the records that matches the filter criteria 
    /// </summary>
    /// <param name="filterCriteria"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<IProduct>> FindPagedProductRecordsAsync(Dictionary<string, IFilterMetaData[]> filterCriteria = null!,
        int pageNumber =1 , int pageSize = 10, CancellationToken cancellationToken = default);

    Task<long> GetProductCountAsync(CancellationToken cancellationToken);
}