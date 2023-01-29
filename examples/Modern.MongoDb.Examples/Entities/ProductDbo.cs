using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Modern.MongoDb.Examples.Entities;

/// <summary>
/// The product entity model
/// </summary>
public class ProductDbo
{
    /// <summary>
    /// Identifier of the entity in the database (_id)
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;

    /// <summary>
    /// Name of the product
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Price of the product
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Count of product items in the storehouse
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Dynamic attributes of the product.<br/>
    /// Every product might have it's own unique attributes
    /// </summary>
    [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)]
    public Dictionary<string, string> Attributes { get; set; } = new();
}