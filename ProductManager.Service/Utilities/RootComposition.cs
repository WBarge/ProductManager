using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Contact.Service.Utilities
{
    /// <summary>
    /// Class RootComposition.
    /// This class is an implementation of the root composition pattern https://freecontent.manning.com/dependency-injection-in-net-2nd-edition-understanding-the-composition-root/
    /// We might want to move this class to a different library (dll) so the service application only depends on that different library and the glue library
    /// </summary>
    public static class RootComposition
    {
        /// <summary>
        /// Configures the di.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        public static void ConfigureDi(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddDbContextPool<ContactDbContext>(builder =>
            //{
            //    builder.UseSqlServer(configuration["ConnectionString"]);
            //});

        }
    }
}