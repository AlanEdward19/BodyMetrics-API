using System.Globalization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace BodyMetricsApi.Infrastructure.Persistence;

public sealed class DateOnlyBsonSerializer : SerializerBase<DateOnly>
{
    private const string Format = "yyyy-MM-dd";

    public override DateOnly Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var stringValue = context.Reader.ReadString();
        return DateOnly.ParseExact(stringValue, Format, CultureInfo.InvariantCulture);
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, DateOnly value)
    {
        context.Writer.WriteString(value.ToString(Format, CultureInfo.InvariantCulture));
    }
}

public static class MongoSerializationBootstrapper
{
    private static readonly object SyncRoot = new();
    private static bool _isConfigured;

    public static void Configure()
    {
        lock (SyncRoot)
        {
            if (_isConfigured)
            {
                return;
            }

            BsonSerializer.RegisterSerializer(new DateOnlyBsonSerializer());
            _isConfigured = true;
        }
    }
}

