// ***********************************************************************
// Author           : Bill Barge
// Created          : 07-12-2024
//
// Last Modified By : Bill Barge
// Last Modified On : 07-12-2024
// ***********************************************************************
// <copyright file="ProductOption.cs" company="N/A">
//     Copyright (c) N/A. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using CrossCutting.Extensions;
using ProductManager.Glue.Interfaces.Models;

namespace ProductManager.Data.EF.Model;

/// <summary>
/// Class ProductOption.
/// Represents an option for a product - can cost extra
/// </summary>
public class ProductOption : IProductOption
{
    /// <summary>
    /// Gets or sets the identifier.
    /// primary identifier for the record
    /// </summary>
    /// <value>The identifier.</value>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the product identifier.
    /// </summary>
    /// <value>The product identifier.</value>
    public Guid ProductId { get; set; }

    /// <summary>
    /// The name backing field
    /// </summary>
    private string _name = string.Empty;
    /// <summary>
    /// The name maximum size
    /// </summary>
    internal const int NAME_MAX_SIZE = 128;
    /// <summary>
    /// Gets or sets the name.
    /// the name of the product
    /// Is limited to 128 characters - will be silently truncated if longer
    /// </summary>
    /// <value>The name.</value>
    public string Name
    {
        get => _name;
        set
        {
            _name = value.Truncate(NAME_MAX_SIZE);
        }
    }

    /// <summary>
    /// The short description backing field
    /// </summary>
    private string _shortDescription = string.Empty;
    /// <summary>
    /// The short description maximum size
    /// </summary>
    internal const int SHORT_DESCRIPTION_MAX_SIZE = 256;
    /// <summary>
    /// Gets or sets the short description.
    /// A short description for the product
    /// Is limited to 256 characters - will be silently truncated if longer
    /// </summary>
    /// <value>The short description.</value>
    public string ShortDescription
    {
        get => _shortDescription;
        set
        {
            _shortDescription = value.Truncate(SHORT_DESCRIPTION_MAX_SIZE);
        }
    }

    /// <summary>
    /// Gets or sets the description.
    /// The full description of the product.
    /// Is optional
    /// </summary>
    /// <value>The description.</value>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the price.
    /// How much the product costs.
    /// </summary>
    /// <value>The price.</value>
    public decimal Price { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="ProductOption" /> is deleted.
    /// </summary>
    /// <value><c>true</c> if deleted; otherwise, <c>false</c>.</value>
    public bool Deleted { get; set; }

    /// <summary>
    /// Gets or sets the created.
    /// represents when the record was created
    /// </summary>
    /// <value>The created.</value>
    public DateTime Created { get; set; }

    /// <summary>
    /// Gets or sets the modified.
    /// represents when the record was last changed
    /// </summary>
    /// <value>The modified.</value>
    public DateTime? Modified { get; set; }

    /// <summary>
    /// Gets or sets the product.
    /// The product the option is for
    /// </summary>
    /// <value>The product.</value>
    public Product Product { get; set; } = null!;
}