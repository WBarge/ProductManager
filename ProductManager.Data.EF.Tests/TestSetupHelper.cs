using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductManager.Data.EF.Model;


namespace ProductManager.Data.EF.Tests
{
    public static class TestSetupHelper
    {
        public static IServiceProvider GetServiceProvider()
        {
            ServiceCollection services = new ServiceCollection();

            services.AddEntityFrameworkInMemoryDatabase()
                .AddDbContext<ProductDbContext>(optionsBuilder =>
                {
                    optionsBuilder.UseInMemoryDatabase("TestDb");
                });

            ServiceProvider serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }

        public static void SeedData(this IServiceScope serviceScope)
        {
            ProductDbContext context = serviceScope.ServiceProvider.GetService<ProductDbContext>() ?? throw new InvalidOperationException();
            Product p = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Test Product",
                ShortDescription = "Test",
                Sku = "T125",
                Description = "A Test Product",
                Price = 12.99M
            };
            context.Products.Add(p);
            Product p1 = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Test2 Product",
                ShortDescription = "Test2",
                Sku = "T124",
                Description = "A Test two Product",
                Price = 12.99M
            };
            context.Products.Add(p1);
            Product p3 = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Test3 Product",
                ShortDescription = "Test3",
                Sku = "T14",
                Description = "A Test three Product",
                Price = 12.99M
            };
            context.Products.Add(p3);
            Product p4 = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Test4 Product",
                ShortDescription = "Test4",
                Sku = "T24",
                Description = "A Test four Product",
                Price = 12.99M
            };
            context.Products.Add(p4);
            Product p5 = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Test5 Product",
                ShortDescription = "Test5",
                Sku = "T5",
                Description = "A Test five Product",
                Price = 15.99M
            };
            context.Products.Add(p5);
            context.SaveChanges();

        }

        public static void RemoveData(this IServiceScope serviceScope)
        {
            ProductDbContext context = serviceScope.ServiceProvider.GetService<ProductDbContext>() ?? throw new InvalidOperationException();
            foreach (Product contextProduct in context.Products)
            {
                context.Remove(contextProduct);
            }
            context.SaveChanges();

        }
    }
}