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
            ProductDbContext context = serviceScope.ServiceProvider.GetService<ProductDbContext>() ??
                                       throw new InvalidOperationException();
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
            ProductDbContext context = serviceScope.ServiceProvider.GetService<ProductDbContext>() ??
                                       throw new InvalidOperationException();
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
            ProductDbContext context = serviceScope.ServiceProvider.GetService<ProductDbContext>() ??
                                       throw new InvalidOperationException();
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
            ProductDbContext context = serviceScope.ServiceProvider.GetService<ProductDbContext>() ??
                                       throw new InvalidOperationException();
            foreach (Product contextProduct in context.Products)
            {
                context.Remove(contextProduct);
            }

            context.SaveChanges();

        }

        public static void SeedDataForOptions(this IServiceScope serviceScope)
        {
            ProductDbContext context = serviceScope.ServiceProvider.GetService<ProductDbContext>() ??
                                       throw new InvalidOperationException();
            Option o = new Option()
            {
                Id = Guid.NewGuid(), Name = "Test Option", Description = "A Test Option", Price = 12.99M
            };
            context.Options.Add(o);
            Option o1 = new Option
            {
                Id = Guid.NewGuid(), Name = "Test2 Option", Description = "A Test two Option", Price = 12.99M
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
                Id = Guid.NewGuid(), Name = "Test3 Option", Description = "A Test three Option", Price = 12.99M
            };
            context.Options.Add(o3);
            Option o4 = new Option
            {
                Id = Guid.NewGuid(), Name = "Test4 Option", Description = "A Test four Option", Price = 12.99M
            };
            context.Options.Add(o4);
            Option o5 = new Option
            {
                Id = Guid.NewGuid(), Name = "Test5 Option", Description = "A Test five Option", Price = 15.99M
            };
            context.Options.Add(o5);
            context.SaveChanges();
        }

        public static void RemoveOptionData(this IServiceScope serviceScope)
        {
            ProductDbContext context = serviceScope.ServiceProvider.GetService<ProductDbContext>() ??
                                       throw new InvalidOperationException();
            foreach (Option contextOption in context.Options)
            {
                context.Remove(contextOption);
            }

            context.SaveChanges();
        }

        public static void SeedDataForProductOptions(this IServiceScope serviceScope)
        {
            ProductDbContext context = serviceScope.ServiceProvider.GetService<ProductDbContext>() ??
                                       throw new InvalidOperationException();

            // Prerequisite products, since a product option must reference a valid product
            Product product1 = new Product
            {
                Id = Guid.NewGuid(),
                Name = "PO Test Product",
                ShortDescription = "POTest",
                Sku = "POT1",
                Description = "A product used for product option tests",
                Price = 9.99M
            };
            context.Products.Add(product1);
            Product product2 = new Product
            {
                Id = Guid.NewGuid(),
                Name = "PO Test Product2",
                ShortDescription = "POTest2",
                Sku = "POT2",
                Description = "A second product used for product option tests",
                Price = 19.99M
            };
            context.Products.Add(product2);

            // Prerequisite options, since a product option must reference a valid option
            Option option1 = new Option
            {
                Id = Guid.NewGuid(),
                Name = "PO Test Option",
                Description = "An option used for product option tests",
                Price = 5.99M
            };
            context.Options.Add(option1);
            Option option2 = new Option
            {
                Id = Guid.NewGuid(),
                Name = "PO Test Option2",
                Description = "A second option used for product option tests",
                Price = 7.99M
            };
            context.Options.Add(option2);

            context.SaveChanges();

            ProductOption po = new ProductOption
            {
                Id = Guid.NewGuid(), ProductId = product1.Id, OptionId = option1.Id, Price = option1.Price
            };
            context.ProductOptions.Add(po);
            ProductOption po1 = new ProductOption
            {
                Id = Guid.NewGuid(), ProductId = product1.Id, OptionId = option2.Id, Price = option2.Price
            };
            context.ProductOptions.Add(po1);
            ProductOption po2 = new ProductOption
            {
                Id = Guid.NewGuid(),
                ProductId = product2.Id,
                OptionId = option1.Id,
                Price = option1.Price,
                Deleted = true
            };
            context.ProductOptions.Add(po2);

            context.SaveChanges();
        }

        public static void RemoveProductOptionData(this IServiceScope serviceScope)
        {
            ProductDbContext context = serviceScope.ServiceProvider.GetService<ProductDbContext>() ??
                                       throw new InvalidOperationException();
            foreach (ProductOption contextProductOption in context.ProductOptions)
            {
                context.Remove(contextProductOption);
            }

            context.SaveChanges();

            // Remove the prerequisite products and options created in SeedDataForProductOptions
            // so this fixture leaves the shared in-memory database clean for other test fixtures.
            foreach (Product contextProduct in context.Products)
            {
                context.Remove(contextProduct);
            }

            foreach (Option contextOption in context.Options)
            {
                context.Remove(contextOption);
            }

            context.SaveChanges();
        }


        public static void SeedDataForProductCharacteristics(this IServiceScope serviceScope)
        {
            ProductDbContext context = serviceScope.ServiceProvider.GetService<ProductDbContext>() ??
                                       throw new InvalidOperationException();

            // Prerequisite products, since a product characteristic must reference a valid product
            Product product1 = new Product
            {
                Id = Guid.NewGuid(),
                Name = "PC Test Product",
                ShortDescription = "PCTest",
                Sku = "PCT1",
                Description = "A product used for product characteristic tests",
                Price = 9.99M
            };
            context.Products.Add(product1);
            Product product2 = new Product
            {
                Id = Guid.NewGuid(),
                Name = "PC Test Product2",
                ShortDescription = "PCTest2",
                Sku = "PCT2",
                Description = "A second product used for product characteristic tests",
                Price = 19.99M
            };
            context.Products.Add(product2);
            context.SaveChanges();

            ProductCharacteristic pc1 = new ProductCharacteristic
            {
                Id = Guid.NewGuid(), ProductId = product1.Id, Name = "Color", CharacteristicValue = "Red"
            };
            context.ProductCharacteristics.Add(pc1);
            ProductCharacteristic pc2 = new ProductCharacteristic
            {
                Id = Guid.NewGuid(), ProductId = product1.Id, Name = "Size", CharacteristicValue = "Large"
            };
            context.ProductCharacteristics.Add(pc2);
            ProductCharacteristic pc3 = new ProductCharacteristic
            {
                Id = Guid.NewGuid(),
                ProductId = product1.Id,
                Name = "Material",
                CharacteristicValue = "Wool",
                Deleted = true
            };
            context.ProductCharacteristics.Add(pc3);
            ProductCharacteristic pc4 = new ProductCharacteristic
            {
                Id = Guid.NewGuid(), ProductId = product2.Id, Name = "Color", CharacteristicValue = "Blue"
            };
            context.ProductCharacteristics.Add(pc4);

            context.SaveChanges();
        }

        public static void RemoveProductCharacteristicData(this IServiceScope serviceScope)
        {
            ProductDbContext context = serviceScope.ServiceProvider.GetService<ProductDbContext>() ??
                                       throw new InvalidOperationException();

            foreach (ProductCharacteristic contextProductCharacteristic in context.ProductCharacteristics)
            {
                context.Remove(contextProductCharacteristic);
            }

            context.SaveChanges();

            // Remove the prerequisite products created in SeedDataForProductCharacteristics
            // so this fixture leaves the shared in-memory database clean for other test fixtures.
            foreach (Product contextProduct in context.Products)
            {
                context.Remove(contextProduct);
            }

            context.SaveChanges();
        }
    }
}