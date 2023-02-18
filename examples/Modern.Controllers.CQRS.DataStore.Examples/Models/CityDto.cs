namespace Modern.Controllers.CQRS.DataStore.Examples.Models;

/// <summary>
/// The city dto model
/// </summary>
public class CityDto
{
    /// <summary>
    /// Identifier of the entity in the database
    /// </summary>
    public int Id { get; set; } = default!;

    /// <summary>
    /// City name
    /// </summary>
    public string Name { get; set; } = default!;
    
    /// <summary>
    /// Country where the city is located
    /// </summary>
    public string Country { get; set; } = default!;

    /// <summary>
    /// Area in square kilometers
    /// </summary>
    public double Area { get; set; }

    /// <summary>
    /// Count of people living the in the city
    /// </summary>
    public int Population { get; set; }

    public override string ToString()
        => $"{Country}, {Name}. {Area:0.00} km2, {Population} citizens";
}