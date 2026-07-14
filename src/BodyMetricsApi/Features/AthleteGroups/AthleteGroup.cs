using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BodyMetricsApi.Features.AthleteGroups;

public sealed class AthleteGroup
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; private set; } = ObjectId.GenerateNewId().ToString();

    public string OwnerUserId { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public List<string> AthleteIds { get; private set; } = [];

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    private AthleteGroup()
    {
    }

    private AthleteGroup(string ownerUserId, string name)
    {
        OwnerUserId = NormalizeRequiredText(ownerUserId, nameof(OwnerUserId));
        Name = NormalizeRequiredText(name, nameof(Name));
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public static AthleteGroup Create(string ownerUserId, string name)
    {
        return new AthleteGroup(ownerUserId, name);
    }

    public void Rename(string name)
    {
        Name = NormalizeRequiredText(name, nameof(Name));
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddMember(string athleteId)
    {
        if (string.IsNullOrWhiteSpace(athleteId))
        {
            throw new ArgumentException("AthleteId is required.", nameof(athleteId));
        }

        if (!AthleteIds.Contains(athleteId))
        {
            AthleteIds.Add(athleteId);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void RemoveMember(string athleteId)
    {
        if (AthleteIds.Remove(athleteId))
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }

    private static string NormalizeRequiredText(string value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{propertyName} is required.", propertyName);
        }

        return value.Trim();
    }
}
