namespace Modern.Cache.InMemory.Examples.Models;

public record Product
{
    /// <summary>
    /// Identifier of the entity
    /// </summary>
    public required int Id { get; init; } = default!;

    /// <summary>
    /// Name of the product
    /// </summary>
    public required string Name { get; init; } = default!;

    /// <summary>
    /// Price of the product
    /// </summary>
    public required decimal Price { get; init; }

    /// <summary>
    /// Count of product items in the storehouse
    /// </summary>
    public required int Quantity { get; init; }
}