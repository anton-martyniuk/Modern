using System.ComponentModel.DataAnnotations.Schema;

namespace Modern.Controllers.DataStore.Cached.Examples.Customized.Entities;

/// <summary>
/// The city entity model
/// </summary>
public class CityDbo
{
    /// <summary>
    /// Identifier of the entity in the database
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
}