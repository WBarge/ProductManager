namespace ProductManager.Glue.Interfaces.Models;

public interface IProductOption
{
    /// <summary>
    /// Gets or sets the identifier.
    /// primary identifier for the record
    /// </summary>
    /// <value>The identifier.</value>
    Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the product identifier.
    /// </summary>
    /// <value>The product identifier.</value>
    Guid ProductId { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// the name of the product
    /// Is limited to 128 characters - will be silently truncated if longer
    /// </summary>
    /// <value>The name.</value>
    string Name { get; set; }

    /// <summary>
    /// Gets or sets the short description.
    /// A short description for the product
    /// Is limited to 256 characters - will be silently truncated if longer
    /// </summary>
    /// <value>The short description.</value>
    string ShortDescription { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// The full description of the product.
    /// Is optional
    /// </summary>
    /// <value>The description.</value>
    string? Description { get; set; }

    /// <summary>
    /// Gets or sets the price.
    /// How much the product costs.
    /// </summary>
    /// <value>The price.</value>
    decimal Price { get; set; }
}