using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BodyMetricsApi.Features.Sports;

public sealed class Sport
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; private set; } = ObjectId.GenerateNewId().ToString();

    public string Name { get; private set; } = string.Empty;

    public List<string> Sectors { get; private set; } = [];

    public List<string> Categories { get; private set; } = [];

    private Sport()
    {
    }

    private Sport(string name, IEnumerable<string> sectors, IEnumerable<string> categories)
    {
        UpdateDetails(name, sectors, categories);
    }

    public static Sport Create(string name, IEnumerable<string> sectors, IEnumerable<string> categories)
    {
        return new Sport(name, sectors, categories);
    }

    public void UpdateDetails(string name, IEnumerable<string> sectors, IEnumerable<string> categories)
    {
        Name = NormalizeRequiredText(name, nameof(Name));
        Sectors = NormalizeDistinctValues(sectors, nameof(Sectors));
        Categories = NormalizeDistinctValues(categories, nameof(Categories));
    }

    public void MergeOptions(IEnumerable<string> sectors, IEnumerable<string> categories)
    {
        UpdateDetails(Name, Sectors.Concat(sectors), Categories.Concat(categories));
    }

    public bool SupportsSector(string sector)
    {
        return Sectors.Any(existing => string.Equals(existing, sector.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public bool SupportsCategory(string category)
    {
        return Categories.Any(existing => string.Equals(existing, category.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    private static string NormalizeRequiredText(string value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{propertyName} is required.", propertyName);
        }

        return value.Trim();
    }

    private static List<string> NormalizeDistinctValues(IEnumerable<string> values, string propertyName)
    {
        var normalized = values
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(value => value)
            .ToList();

        if (normalized.Count == 0)
        {
            throw new ArgumentException($"At least one {propertyName} value is required.", propertyName);
        }

        return normalized;
    }
}

