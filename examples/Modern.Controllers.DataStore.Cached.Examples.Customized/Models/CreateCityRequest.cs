namespace Modern.Controllers.DataStore.Cached.Examples.Customized.Models;

/// <summary>
/// The create city request model
/// </summary>
public class CreateCityRequest
{
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
}