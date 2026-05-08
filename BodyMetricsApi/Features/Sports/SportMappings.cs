namespace BodyMetricsApi.Features.Sports;

public static class SportMappings
{
    public static SportResponse ToResponse(this Sport sport)
    {
        return new SportResponse(sport.Id, sport.Name, sport.Sectors, sport.Categories);
    }
}


