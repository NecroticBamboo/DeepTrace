using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace PrometheusAPI;

public static class JsonSetializerSetup
{
    private static JsonSerializerOptions _options = new JsonSerializerOptions
    {
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        NumberHandling =
                    JsonNumberHandling.AllowReadingFromString |
                    JsonNumberHandling.WriteAsString,
        PropertyNameCaseInsensitive = true
    };

    public static JsonSerializerOptions Options => _options;
}
