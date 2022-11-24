using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using HotChocolate.Subscriptions.Redis.Properties;

namespace HotChocolate.Subscriptions.Redis;

internal sealed class DefaultJsonMessageSerializer : IMessageSerializer
{
    private const string _completed = "{\"isCompletedMessage\":true}";
    private readonly JsonSerializerOptions _options =
        new(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

    public string CompleteMessage => _completed;

    public string Serialize<TMessage>(TMessage message)
        => JsonSerializer.Serialize(message, _options);

    public TMessage Deserialize<TMessage>(string serializedMessage)
    {
        var result = JsonSerializer.Deserialize<TMessage>(serializedMessage, _options);

        if (result is null)
        {
            throw new InvalidOperationException(
                Resources.JsonMessageSerializer_Deserialize_MessageIsNull);
        }

        return result;
    }
}