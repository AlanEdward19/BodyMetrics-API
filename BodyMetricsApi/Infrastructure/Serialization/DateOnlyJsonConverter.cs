using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BodyMetricsApi.Infrastructure.Serialization;

public sealed class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    public const string Format = "yyyy-MM-dd";

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new JsonException("A non-empty date string is required.");
        }

        if (!DateOnly.TryParseExact(value, Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOnly))
        {
            throw new JsonException($"Dates must use the {Format} format.");
        }

        return dateOnly;
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(Format, CultureInfo.InvariantCulture));
    }
}

