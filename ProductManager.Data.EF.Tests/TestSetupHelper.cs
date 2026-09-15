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
            Product p = new Product()
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
            Product p2 = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Test2 Product",
                ShortDescription = "Test2",
                Sku = "T124",
                Description = "A Test two Product",
                Price = 12.99M,
                Deleted = true
            };
            context.Products.Add(p2);
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

        public static void SeedDataForCharacteristics(this IServiceScope serviceScope)
        {
            ProductDbContext context = serviceScope.ServiceProvider.GetService<ProductDbContext>() ?? throw new InvalidOperationException();
            // Create Characteristics
            Characteristic characteristic1 = new Characteristic
            {
                Id = Guid.NewGuid(),
                Name = "Color",
                Values = new List<CharacteristicValue>
                {
                    new CharacteristicValue { Id = Guid.NewGuid(), Value = "Red" },
                    new CharacteristicValue { Id = Guid.NewGuid(), Value = "Blue" },
                    new CharacteristicValue { Id = Guid.NewGuid(), Value = "Green" }
                }
            };
            Characteristic characteristic2 = new Characteristic
            {
                Id = Guid.NewGuid(),
                Name = "Size",
                Values = new List<CharacteristicValue>
                {
                    new CharacteristicValue { Id = Guid.NewGuid(), Value = "Small" },
                    new CharacteristicValue { Id = Guid.NewGuid(), Value = "Medium" },
                    new CharacteristicValue { Id = Guid.NewGuid(), Value = "Large" }
                }
            };
            Characteristic characteristic3 = new Characteristic
            {
                Id = Guid.NewGuid(),
                Name = "Material",
                Values = new List<CharacteristicValue>
                {
                    new CharacteristicValue { Id = Guid.NewGuid(), Value = "Cotton" },
                    new CharacteristicValue { Id = Guid.NewGuid(), Value = "Polyester" },
                    new CharacteristicValue { Id = Guid.NewGuid(), Value = "Wool" }
                }
            };
            // Add Characteristics to the context
            context.Characteristics.Add(characteristic1);
            context.Characteristics.Add(characteristic2);
            context.Characteristics.Add(characteristic3);
            // Save changes to the database
            context.SaveChanges();
        }

        public static void RemoveCharacteristicData(this IServiceScope serviceScope)
        {
            ProductDbContext context = serviceScope.ServiceProvider.GetService<ProductDbContext>() ?? throw new InvalidOperationException();
            // Retrieve all Characteristics and their associated CharacteristicValues
            List<Characteristic> characteristics = context.Characteristics.Include(c => c.Values).ToList();
            // Remove associated CharacteristicValues first
            foreach (Characteristic characteristic in characteristics)
            {
                if (characteristic.Values!.Any())
                {
                    context.CharacteristicValues.RemoveRange(characteristic.Values!);
                }
            }
            // Remove Characteristics
            context.Characteristics.RemoveRange(characteristics);
            // Save changes to the database
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

        public static void SeedDataForOptions(this IServiceScope serviceScope)
        {
            ProductDbContext context = serviceScope.ServiceProvider.GetService<ProductDbContext>() ?? throw new InvalidOperationException();
            Option o = new Option()
            {
                Id = Guid.NewGuid(),
                Name = "Test Option",
                Description = "A Test Option",
                Price = 12.99M
            };
            context.Options.Add(o);
            Option o1 = new Option
            {
                Id = Guid.NewGuid(),
                Name = "Test2 Option",
                Description = "A Test two Option",
                Price = 12.99M
            };
            context.Options.Add(o1);
            Option o2 = new Option
            {
                Id = Guid.NewGuid(),
                Name = "Test2 Option",
                Description = "A Test two Option",
                Price = 12.99M,
                Deleted = true
            };
            context.Options.Add(o2);
            Option o3 = new Option
            {
                Id = Guid.NewGuid(),
                Name = "Test3 Option",
                Description = "A Test three Option",
                Price = 12.99M
            };
            context.Options.Add(o3);
            Option o4 = new Option
            {
                Id = Guid.NewGuid(),
                Name = "Test4 Option",
                Description = "A Test four Option",
                Price = 12.99M
            };
            context.Options.Add(o4);
            Option o5 = new Option
            {
                Id = Guid.NewGuid(),
                Name = "Test5 Option",
                Description = "A Test five Option",
                Price = 15.99M
            };
            context.Options.Add(o5);
            context.SaveChanges();
        }

        public static void RemoveOptionData(this IServiceScope serviceScope)
        {
            ProductDbContext context = serviceScope.ServiceProvider.GetService<ProductDbContext>() ?? throw new InvalidOperationException();
            foreach (Option contextOption in context.Options)
            {
                context.Remove(contextOption);
            }
            context.SaveChanges();
        }
    }
}