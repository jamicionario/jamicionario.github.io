
using System.Globalization;
using Newtonsoft.Json;
using NodaTime;
using NodaTime.Serialization.JsonNet;

namespace ScoresProcessor.Helpers;

public static class JsonHelper
{
    private static readonly JsonSerializerSettings Settings = new()
    {
        Culture = CultureInfo.InvariantCulture,
    };

    static JsonHelper()
    {
        Settings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
    }

    public static string Serialize<T>(this T toSerialize)
    {
        string json = JsonConvert.SerializeObject(toSerialize, Formatting.Indented, Settings);
        return json;
    }

    public static T? Deserialize<T>(string json)
    {
        T? parsed = JsonConvert.DeserializeObject<T>(json, Settings);
        return parsed;
    }
}