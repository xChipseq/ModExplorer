using System.Collections.Generic;
using BepInEx.Configuration;
using UnityEngine;

namespace ModExplorer.Data;

public class ModData
{
    public string Name { get; init; }
    public string ID { get; init; }
    public string Description { get; init; }
    public string[] Tags { get; init; } = [];
    public string Version { get; init; }
    public string License { get; init; }
    public string[] Authors { get; init; } = [];
    public string[] Dependencies { get; init; } = [];
    public Dictionary<string, string> Links { get; init; } = [];
    public Sprite Icon { get; set; }
    
    public ConfigFile ConfigFile { get; init; }
}