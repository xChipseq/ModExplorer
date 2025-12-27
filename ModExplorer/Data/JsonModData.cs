using System.Collections.Generic;

namespace ModExplorer.Data;

public class JsonModData
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string[] Tags { get; set; } = [];
    public string License { get; set; }
    public string[] Authors { get; set; } = [];
    public Dictionary<string, string> Links { get; set; } = [];
    public string Icon { get; set; }
}