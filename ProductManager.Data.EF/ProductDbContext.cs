// ***********************************************************************
// Author           : Bill Barge
// Created          : 07-10-2024
//
// Last Modified By : Bill Barge
// Last Modified On : 07-12-2024
// ***********************************************************************
// <copyright file="ProductDbContext.cs" company="N/A">
//     Copyright (c) N/A. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using ProductManager.Data.EF.Model;

namespace ProductManager.Data.EF;

/// <summary>
/// Class ProductDbContext.
/// Implements the <see cref="DbContext" />
/// </summary>
/// <seealso cref="DbContext" />
public class ProductDbContext : DbContext
{

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductDbContext" /> class.
    /// </summary>
    /// <param name="options">The options.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Will always have options when creating the db context")]
    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options) {}

    /// <summary>
    /// Override this method to further configure the model that was discovered by convention from the entity types
    /// exposed in <see cref="T:Microsoft.EntityFrameworkCore.DbSet`1" /> properties on your derived context. The resulting model may be cached
    /// and re-used for subsequent instances of your derived context.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context. Databases (and other extensions) typically
    /// define extension methods on this object that allow you to configure aspects of the model that are specific
    /// to a given database.</param>
    /// <remarks><para>
    /// If a model is explicitly set on the options for this context (via <see cref="M:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseModel(Microsoft.EntityFrameworkCore.Metadata.IModel)" />)
    /// then this method will not be run. However, it will still run when creating a compiled model.
    /// </para>
    /// <para>
    /// See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see> for more information and
    /// examples.
    /// </para></remarks>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    /// <summary>
    /// Gets or sets the products.
    /// </summary>
    /// <value>The products.</value>
    public DbSet<Product> Products { get; set; }

    /// <summary>
    /// Gets or sets the options.
    /// </summary>
    /// <value>The options.</value>
    public DbSet<ProductOption> Options { get; set; }

    /// <summary>
    /// Gets or sets the characteristics.
    /// </summary>
    /// <value>The characteristics.</value>
    public DbSet<ProductCharacteristic> Characteristics { get; set; }

    /// <summary>
    /// Gets or sets the sells.
    /// </summary>
    /// <value>The sells.</value>
    public DbSet<ProductSell> Sells { get; set; }

}