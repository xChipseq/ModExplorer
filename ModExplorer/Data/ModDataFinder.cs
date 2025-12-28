using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using ModExplorer.Utilities;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace ModExplorer.Data;

public static class ModDataFinder
{
    private static List<ModData> _mods { get; set; } = new();
    public static IReadOnlyList<ModData> Mods => _mods; 

    internal static void Initialize()
    {
        var plugins = IL2CPPChainloader.Instance.Plugins;
        foreach (var (id, info) in plugins)
        {
            var assembly = info.Instance.GetType().Assembly;
            MiraCompatibility.ProcessPlugin(info, assembly);
            
            try
            {
                JsonModData jsonData = null;
                foreach (var name in assembly.GetManifestResourceNames())
                {
                    if (name.EndsWith(".modinfo.json"))
                    {
                        using var stream = assembly.GetManifestResourceStream(name)!;
                        using StreamReader sr = new StreamReader(stream);
                        string json = sr.ReadToEnd();
                        jsonData = JsonSerializer.Deserialize<JsonModData>(json);
                    }
                }

                var dependencies = info.Dependencies
                    .Where(x => x.Flags == BepInDependency.DependencyFlags.HardDependency)
                    .Select(x => x.DependencyGUID)
                    .ToArray();
                var data = new ModData
                {
                    Name = jsonData?.Name ?? info.Metadata.Name,
                    ID = id,
                    Authors = jsonData?.Authors ?? [],
                    Description = jsonData?.Description ?? string.Empty,
                    Dependencies = dependencies,
                    License = jsonData?.License ?? string.Empty,
                    Links = jsonData?.Links ?? [],
                    Tags = jsonData?.Tags ?? [],
                    Version = info.Metadata.Version.WithoutBuild().ToString(),
                    ConfigFile = (info.Instance as BasePlugin).Config
                };

                if (jsonData != null && TryGetIcon(assembly, jsonData, out var icon))
                {
                    data.Icon = icon;
                }
                
                _mods.Add(data);
                Logger<ModExplorerPlugin>.Info($"ModData for {id} created");
            }
            catch (Exception e)
            {
                Logger<ModExplorerPlugin>.Error($"Unable to register ModData for {id}\n{e.ToString()}");
            }
        }
    }

    public static ModData GetModData(string id)
    {
        return _mods.First(x => x.ID == id);
    }

    private static bool TryGetIcon(Assembly assembly, JsonModData data, out Sprite icon)
    {
        icon = null;
        if (data.Icon.IsNullOrWhiteSpace())
            return false;

        try
        {
            icon = SpriteTools.LoadSpriteFromPath(data.Icon, assembly, 100f);
        }
        catch
        {
            return false;
        }

        return true;
    }
}