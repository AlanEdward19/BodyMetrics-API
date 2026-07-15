using BodyMetricsApi.Features.Athletes.Shared.Enums;

namespace BodyMetricsApi.Features.Athletes.Shared;

// Shared in-memory filter/paginate logic used both by the standalone-athletes repository
// query and by handlers that need to filter athletes merged in from group membership.
public static class AthleteFilter
{
    public static IEnumerable<Athlete> Apply(
        IEnumerable<Athlete> athletes,
        string? fullName,
        string? sportId,
        string? sector,
        string? category,
        Phase? phase)
    {
        var filtered = athletes;

        if (!string.IsNullOrWhiteSpace(fullName))
        {
            var normalizedFullName = NormalizeSearchTerm(fullName);
            filtered = filtered.Where(athlete => MatchesFullNameSearch(athlete.FullName, normalizedFullName));
        }

        if (!string.IsNullOrWhiteSpace(sportId))
        {
            var normalizedSportId = sportId.Trim();
            filtered = filtered.Where(athlete => string.Equals(athlete.SportId, normalizedSportId, StringComparison.Ordinal));
        }

        if (!string.IsNullOrWhiteSpace(sector))
        {
            var normalizedSector = sector.Trim();
            filtered = filtered.Where(athlete => string.Equals(athlete.Sector, normalizedSector, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            var normalizedCategory = category.Trim();
            filtered = filtered.Where(athlete => string.Equals(athlete.Category, normalizedCategory, StringComparison.OrdinalIgnoreCase));
        }

        if (phase.HasValue)
        {
            filtered = filtered.Where(athlete => athlete.Phase == phase.Value);
        }

        return filtered;
    }

    public static List<Athlete> Paginate(IEnumerable<Athlete> athletes, int page, int pageSize, out int totalCount)
    {
        var ordered = athletes.OrderBy(athlete => athlete.FullName).ToList();
        totalCount = ordered.Count;
        return ordered.Skip((page - 1) * pageSize).Take(pageSize).ToList();
    }

    private static bool MatchesFullNameSearch(string fullName, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return true;
        }

        var normalizedFullName = NormalizeSearchTerm(fullName);
        if (normalizedFullName.StartsWith(searchTerm, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return normalizedFullName
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Any(token => token.StartsWith(searchTerm, StringComparison.OrdinalIgnoreCase));
    }

    private static string NormalizeSearchTerm(string value)
    {
        return string.Join(' ', value
            .Trim()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }
}
