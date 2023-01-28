namespace Modern.LiteDb.Async.Examples.Entities;

/// <summary>
/// The car entity model
/// </summary>
public class CarDbo
{
    /// <summary>
    /// Identifier of the entity in the database (_id)
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Name of the manufacturer
    /// </summary>
    public string Manufacturer { get; set; } = default!;

    /// <summary>
    /// Model name
    /// </summary>
    public string Model { get; set; } = default!;

    /// <summary>
    /// Description
    /// </summary>
    public string Engine { get; set; } = default!;
    
    /// <summary>
    /// Type of the drive: FWD, AWD, RWD
    /// </summary>
    public string Drive { get; set; } = default!;

    /// <summary>
    /// The year the car was manufactured
    /// </summary>
    public int YearOfProduction { get; set; }

    /// <summary>
    /// Price
    /// </summary>
    public decimal Price { get; set; }
}