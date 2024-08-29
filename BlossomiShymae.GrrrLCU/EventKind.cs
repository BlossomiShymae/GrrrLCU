using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BlossomiShymae.GrrrLCU;

namespace BlossomiShymae.GrrrLCU
{
    /// <summary>
    /// The kind of an event message including prefix and path.
    /// </summary>
    [JsonConverter(typeof(EventKindJsonConverter))]
    public record EventKind
    {
        /// <summary>
        /// The prefix of kind for the associated event e.g. "OnJsonApiEvent".
        /// </summary>
        public string Prefix { get; init; } = "OnJsonApiEvent";

        /// <summary>
        /// The path of kind for the associated event e.g. "/lol-summoner/v1/current-summoner".
        /// </summary>
        public string? Path { get; init; }

        /// <summary>
        /// The formatted value of the kind used for the internal Websocket client e.g. "OnJsonApiEvent_lol-summoner_v1_current-summoner".
        /// </summary>
        public string Value
        {
            get 
            {
                if (string.IsNullOrWhiteSpace(Path)) 
                    return Prefix;
                return $"{Prefix}_{Path.Replace("/", "_")}";
            }
        }
    }
}