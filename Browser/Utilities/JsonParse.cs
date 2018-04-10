// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using QuickType;
//
//    var welcome = Welcome.FromJson(jsonString);

namespace Utilities
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class JsonParse
    {
        [JsonProperty("Id")]
        public long Id { get; set; }

        [JsonProperty("Value")]
        public string Value { get; set; }

        [JsonProperty("Path")]
        public string Path { get; set; }

        [JsonProperty("Time")]
        public System.DateTimeOffset Time { get; set; }
    }

    public partial class JsonParse
    {
        public static JsonParse[] FromJson(string json) => JsonConvert.DeserializeObject<JsonParse[]>(json, Utilities.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this JsonParse[] self) => JsonConvert.SerializeObject(self, Utilities.Converter.Settings);
    }

    internal class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
