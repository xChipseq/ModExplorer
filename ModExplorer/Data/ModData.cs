using System.Collections.Generic;
using System.Text.Json.Serialization;
using UnityEngine;

namespace ModExplorer.Data;

public class ModData
{
    public string Name { get; set; }
    [JsonIgnore]
    public string ID { get; set; }
    public string Description { get; set; }
    public string[] Tags { get; set; } = [];
    [JsonIgnore]
    public string Version { get; set; }
    public string License { get; set; }
    public string[] Authors { get; set; } = [];
    [JsonIgnore]
    public string[] Dependencies { get; set; } = [];
    public Dictionary<string, string> Links { get; set; } = [];
    
    [JsonPropertyName("Icon")]
    public string IconResource { get; set; }
    [JsonIgnore]
    public Sprite Icon { get; set; }
}