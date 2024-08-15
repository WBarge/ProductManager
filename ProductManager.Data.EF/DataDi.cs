using Microsoft.Extensions.DependencyInjection;
using ProductManager.Data.EF.Repos;
using ProductManager.Glue.Interfaces.Repos;

namespace ProductManager.Data.EF;

/// <summary>
/// Class DiHelper.
/// </summary>
public static class DataDi
{
    /// <summary>
    /// Configures the di.
    /// </summary>
    /// <param name="services">The services.</param>
    public static void ConfigureDi(IServiceCollection services)
    {
        services.AddTransient<IProductRepo, ProductRepo>();
    }
}