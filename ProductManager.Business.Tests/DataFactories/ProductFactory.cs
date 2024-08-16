﻿using ProductManager.Glue.Interfaces.Models;

namespace ProductManager.Business.Tests.DataFactories;

public static class ProductFactory
{
    public static IEnumerable<IProduct> BuildShortProductList()
    {
        List<IProduct> returnValue =
        [
            new ShortP
            (
                Guid.NewGuid(),
                "test",
                "test short description",
                "L1253",
                12.99M
            ),
            new ShortP
            (
                Guid.NewGuid(),
                "test 2",
                "test 2 short description",
                "M1253",
                5.99M
            )

        ];
        return returnValue;
    }
}

internal class ShortP(Guid id, string name, string shortDescription, string sku, decimal price) : IProduct
{
    public Guid Id { get; set; } = id;
    public string Name { get; set; } = name;
    public string ShortDescription { get; set; } = shortDescription;
    public string Sku { get; set; } = sku;
    public decimal Price { get; set; } = price;
    public string? Description { get; set; }
}