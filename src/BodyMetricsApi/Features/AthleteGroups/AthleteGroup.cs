using BodyMetricsApi.Features.Athletes;
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

    // Athletes are embedded here (not referenced by id) so a group and its members
    // live in one Mongo document - moving an athlete in/out physically relocates the
    // document instead of maintaining a relational foreign key inside MongoDB.
    public List<Athlete> Members { get; private set; } = [];

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

    public void AddMember(Athlete athlete)
    {
        ArgumentNullException.ThrowIfNull(athlete);

        if (Members.All(member => member.Id != athlete.Id))
        {
            Members.Add(athlete);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public Athlete? RemoveMember(string athleteId)
    {
        var member = Members.FirstOrDefault(m => m.Id == athleteId);
        if (member is null)
        {
            return null;
        }

        Members.Remove(member);
        UpdatedAt = DateTime.UtcNow;
        return member;
    }

    public void ReplaceMember(Athlete athlete)
    {
        var index = Members.FindIndex(m => m.Id == athlete.Id);
        if (index < 0)
        {
            throw new InvalidOperationException($"Athlete '{athlete.Id}' is not a member of this group.");
        }

        Members[index] = athlete;
        UpdatedAt = DateTime.UtcNow;
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
