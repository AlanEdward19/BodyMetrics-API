using BodyMetricsApi.Features.Athletes.PhysicalAssessments;
using BodyMetricsApi.Features.Athletes.Shared.Enums;
using BodyMetricsApi.Features.Athletes.Shared.ValueObjects;
using BodyMetricsApi.Features.Sports;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BodyMetricsApi.Features.Athletes;

public sealed class Athlete
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; private set; } = ObjectId.GenerateNewId().ToString();

    public string FullName { get; private set; } = string.Empty;

    public string OwnerUserId { get; private set; } = string.Empty;

    [BsonRepresentation(BsonType.ObjectId)]
    public string SportId { get; private set; } = string.Empty;

    public string SportName { get; private set; } = string.Empty;

    public string Sector { get; private set; } = string.Empty;

    [BsonRepresentation(BsonType.String)]
    public Phase Phase { get; private set; }

    public string Category { get; private set; } = string.Empty;

    [BsonRepresentation(BsonType.String)]
    public Sex Sex { get; private set; }

    [BsonRepresentation(BsonType.String)]
    public Ethnicity Ethnicity { get; private set; }

    public DateOnly BirthDate { get; private set; }

    public ProfilePhotoReferenceValueObject? ProfilePhoto { get; private set; }

    public List<PhysicalAssessment> PhysicalAssessments { get; private set; } = [];

    private Athlete()
    {
    }

    private Athlete(
        string ownerUserId,
        string fullName,
        Sport sport,
        string sector,
        Phase phase,
        string category,
        Sex sex,
        Ethnicity ethnicity,
        DateOnly birthDate,
        IEnumerable<PhysicalAssessment> physicalAssessments,
        ProfilePhotoReferenceValueObject? profilePhoto)
    {
        OwnerUserId = NormalizeRequiredText(ownerUserId, nameof(OwnerUserId));
        Update(fullName, sport, sector, phase, category, sex, ethnicity, birthDate, physicalAssessments, profilePhoto);
    }

    public static Athlete Create(
        string ownerUserId,
        string fullName,
        Sport sport,
        string sector,
        Phase phase,
        string category,
        Sex sex,
        Ethnicity ethnicity,
        DateOnly birthDate,
        IEnumerable<PhysicalAssessment> physicalAssessments,
        ProfilePhotoReferenceValueObject? profilePhoto)
    {
        return new Athlete(ownerUserId, fullName, sport, sector, phase, category, sex, ethnicity, birthDate, physicalAssessments, profilePhoto);
    }

    public void Update(
        string fullName,
        Sport sport,
        string sector,
        Phase phase,
        string category,
        Sex sex,
        Ethnicity ethnicity,
        DateOnly birthDate,
        IEnumerable<PhysicalAssessment> physicalAssessments,
        ProfilePhotoReferenceValueObject? profilePhoto)
    {
        FullName = NormalizeRequiredText(fullName, nameof(FullName));
        SportId = sport.Id;
        SportName = sport.Name;
        Sector = NormalizeRequiredText(sector, nameof(Sector));
        Category = NormalizeRequiredText(category, nameof(Category));
        if (!sport.SupportsSector(Sector))
        {
            throw new ArgumentException($"Sector '{Sector}' is not valid for sport '{sport.Name}'.", nameof(Sector));
        }

        if (!sport.SupportsCategory(Category))
        {
            throw new ArgumentException($"Category '{Category}' is not valid for sport '{sport.Name}'.", nameof(Category));
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (birthDate > today)
        {
            throw new ArgumentException("BirthDate cannot be in the future.", nameof(BirthDate));
        }

        Phase = phase;
        Sex = sex;
        Ethnicity = ethnicity;
        BirthDate = birthDate;
        PhysicalAssessments = NormalizeAssessments(physicalAssessments);
        ProfilePhoto = profilePhoto;
    }

    public void SetProfilePhoto(ProfilePhotoReferenceValueObject? profilePhoto)
    {
        ProfilePhoto = profilePhoto;
    }

    private static string NormalizeRequiredText(string value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{propertyName} is required.", propertyName);
        }

        return value.Trim();
    }

    private static List<PhysicalAssessment> NormalizeAssessments(IEnumerable<PhysicalAssessment> physicalAssessments)
    {
        var normalized = physicalAssessments
            .OrderBy(assessment => assessment.AssessmentDate)
            .ToList();

        var duplicateDates = normalized
            .GroupBy(assessment => assessment.AssessmentDate)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();

        if (duplicateDates.Count > 0)
        {
            throw new ArgumentException("Physical assessments must have unique assessment dates.", nameof(physicalAssessments));
        }

        return normalized;
    }
}