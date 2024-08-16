namespace ProductManager.Glue.Interfaces.Models;

/// <summary>
/// Interface IFullProduct
/// Extends the <see cref="IShortProduct" />
/// </summary>
/// <seealso cref="IShortProduct" />
public interface IProduct :IShortProduct
{
    /// <summary>
    /// Gets or sets the description.
    /// The full description of the product.
    /// Is optional
    /// </summary>
    /// <value>The description.</value>
    string? Description { get; set; }

    /// <summary>
    /// Gets or sets the options.
    /// represents the options available for the product
    /// </summary>
    /// <value>The options.</value>
//    ICollection<ProductOption>? Options { get; set; }

    /// <summary>
    /// Gets or sets the characteristics.
    /// represents the characteristics of the product
    /// </summary>
    /// <value>The characteristics.</value>
//    ICollection<ProductCharacteristic>? Characteristics { get; set; }

    /// <summary>
    /// Gets or sets the reductions.
    /// Represents time periods in which a product is on sale
    /// </summary>
    /// <value>The reductions.</value>
//    ICollection<ProductSell>? Reductions { get; set; }
}