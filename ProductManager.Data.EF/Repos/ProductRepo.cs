using CrossCutting.Extensions;
using Microsoft.EntityFrameworkCore;
using ProductManager.Data.EF.Helpers;
using ProductManager.Data.EF.Model;
using ProductManager.Data.EF.Transformers;
using ProductManager.Data.EF.Transformers.InternalModels;
using ProductManager.Glue.Interfaces.Models;
using ProductManager.Glue.Interfaces.Repos;
using System.Reflection.PortableExecutable;

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
        list[0] = new FilterCriteria()
        {
            SearchValue = "False",
            MatchMode = FilteringEngine.EQUALS_COMPARISON,
            LogicalOperator = FilteringEngine.AND_LOGICAL_OPERATOR
        };
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
    public async Task<long> GetProductCountAsync(CancellationToken cancellationToken = default)
    {
        long result = 0;
        await Task.Run(() =>
        {
            result = DbContext.Products.Where(p => p.Deleted == false).LongCount();
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
    public async Task<Guid> AddMinimumProductAsync(IProduct product, CancellationToken cancellationToken = default)
    {
        Product p = Create();
        p.Name = product.Name;
        p.Sku = product.Sku;
        p.ShortDescription = product.ShortDescription;
        p.Price = product.Price;
        p.Cost = product.Cost;
        p.Description = product.Description;
        await InsertAsync(p, cancellationToken);
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

    /// <summary>
    /// Gets the product with all details
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IFullProduct?> GetProductAsync(Guid id, CancellationToken cancellationToken)
    {
        IFullProduct? returnValue = null;
        Product? p = await DbContext.Products
            .Include(x => x.Characteristics)
            .Include(x => x.Options)
            .ThenInclude(x => x.Option)
            .Include(x => x.Reductions)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (p.IsNotEmpty())
        {
            returnValue = ProductTransformer.Transform(p);
        }

        return returnValue;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="product"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task UpdateProductAsync(IFullProduct product, CancellationToken cancellationToken = default)
    {
        try
        {

            Product? existingProduct = await DbContext.Products
                .Include(p=>p.Characteristics)
                .Include(p=>p.Options)
                .Include(p=>p.Reductions)
                .FirstOrDefaultAsync(p => p.Id == product.Id, cancellationToken: cancellationToken);
            if (existingProduct == null)
            {
                return;
            }

            //odd but EF is saying I am modifing the enum if I loop thought the passed in collect when I add the dbcontext collection.
            List<IProductCharacteristic> loopCharacteristics = product.Characteristics.ToList(); 
            foreach (IProductCharacteristic productCharacteristic in loopCharacteristics)
            {

                ProductCharacteristic? newProductCharacteristic =
                    existingProduct.Characteristics.FirstOrDefault(c => c.Id == productCharacteristic.Id);
                if (newProductCharacteristic == null)
                {
                    newProductCharacteristic = new ProductCharacteristic();
                    DbContext.ProductCharacteristics.Add(newProductCharacteristic);
                }

                newProductCharacteristic.ProductId = existingProduct!.Id;
                newProductCharacteristic.Name = productCharacteristic.Name;
                newProductCharacteristic.CharacteristicValue = productCharacteristic.CharacteristicValue;
            }

            foreach (ProductCharacteristic productCharacteristic in existingProduct.Characteristics)
            {
                IProductCharacteristic? submittedCharacteristic = loopCharacteristics.FirstOrDefault(c => c.Id == productCharacteristic.Id);
                if (submittedCharacteristic == null)
                {
                    productCharacteristic.Deleted = true;
                }
            }

            //odd but EF is saying I am modifing the enum if I loop thought the passed in collect when I add the dbcontext collection.
            List<IFullProductOption> loopOptions = product.Options.ToList(); 
            foreach (IFullProductOption fullProductOption in loopOptions)
            {
                ProductOption? existingProductOption = existingProduct.Options.FirstOrDefault(po => po.Id == fullProductOption.Id);
                if (existingProductOption == null)
                {
                    existingProductOption = new ProductOption();
                    DbContext.ProductOptions.Add(existingProductOption);

                }
                Option? option = await DbContext.Options.FirstOrDefaultAsync(o => o.Id == fullProductOption.OptionId,
                    cancellationToken: cancellationToken);
                if (option == null)
                {
                    continue;
                }

                existingProductOption.OptionId = option.Id;
                existingProductOption.ProductId = existingProduct.Id;
                existingProductOption.Price = fullProductOption.Price > 0 ? fullProductOption.Price : option.Price;
                existingProductOption.Cost = option.Cost;
                existingProductOption.Estimated = option.Estimated;
                existingProductOption.Deleted = false;
            }

            foreach (ProductOption existingProductOption in existingProduct.Options)
            {
                var submittedOption = loopOptions.FirstOrDefault(o => o.Id == existingProductOption.Id);
                if (submittedOption == null)
                {
                    existingProductOption.Deleted = true;
                }
            }




            List<IProductSell> loopSells = product.Sells.ToList();
            foreach (var newSell in loopSells)
            {
                if (newSell.Start.IsEmpty() || newSell.End.IsEmpty())
                {
                    continue;
                }

                ProductSell? productSell = existingProduct.Reductions.FirstOrDefault(po => po.Id == newSell.Id);
                if (productSell == null)
                {
                    productSell = new ProductSell();
                    DbContext.Sells.Add(productSell);

                }

                productSell.ProductId = existingProduct.Id;
                productSell.Price = newSell.Price > 0 ? newSell.Price : existingProduct.Price;
                productSell.Start = newSell.Start;
                productSell.End = newSell.End;
                productSell.Deleted = false;
            }

            foreach (ProductSell existingProductReduction in existingProduct.Reductions)
            {
                var submittedSell = loopSells.FirstOrDefault(s => s.Id == existingProductReduction.Id);
                {
                    existingProductReduction.Deleted = true;
                }
            }

            existingProduct.Name = product.Name ?? existingProduct.Name;
            existingProduct.Sku = product.Sku ?? existingProduct.Sku;
            existingProduct.ShortDescription = product.ShortDescription ?? existingProduct.ShortDescription;
            existingProduct.Description = product.Description ?? existingProduct.Description;
            existingProduct.Price = product.Price <= 0 ? existingProduct.Price : product.Price;
            existingProduct.Cost = product.Cost <= 0 ? existingProduct.Cost : product.Cost;
            existingProduct.Cost = product.Cost <= 0 ? existingProduct.Cost : product.Cost;
            existingProduct.Estimated = product.Estimated <= 0 ? existingProduct.Estimated : product.Estimated;
            await SaveAsync(cancellationToken);


            Update(existingProduct);
            await SaveAsync(cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }
}