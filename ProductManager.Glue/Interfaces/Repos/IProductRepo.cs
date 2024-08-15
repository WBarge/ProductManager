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
    Task<IEnumerable<IShortProduct>> FindPagedShortProductRecordsAsync(Dictionary<string, IFilterMetaData[]> filterCriteria,
        int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}