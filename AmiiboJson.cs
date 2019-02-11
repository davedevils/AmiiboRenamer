
namespace AmiiboJson
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class AmiiboHelp
    {
        [JsonProperty("amiibo_series", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> AmiiboSeries { get; set; }

        [JsonProperty("amiibos", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, Amiibo> Amiibos { get; set; }

        [JsonProperty("characters", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Characters { get; set; }

        [JsonProperty("game_series", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> GameSeries { get; set; }

        [JsonProperty("types", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Types { get; set; }
    }
    public partial class Amiibo
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("release", NullValueHandling = NullValueHandling.Ignore)]
        public Release Release { get; set; }
    }

    public partial class Release
    {
        [JsonProperty("au")]
        public DateTimeOffset? Au { get; set; }

        [JsonProperty("eu")]
        public DateTimeOffset? Eu { get; set; }

        [JsonProperty("jp")]
        public DateTimeOffset? Jp { get; set; }

        [JsonProperty("na")]
        public DateTimeOffset? Na { get; set; }
    }

    public partial class AmiiboHelp
    {
        public static AmiiboHelp FromJson(string json) => JsonConvert.DeserializeObject<AmiiboHelp>(json, AmiiboJson.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this AmiiboHelp self) => JsonConvert.SerializeObject(self, AmiiboJson.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
