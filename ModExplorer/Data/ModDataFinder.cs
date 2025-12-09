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
using Object = UnityEngine.Object;

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
            try
            {
                ModData data = null;
                var assembly = info.Instance.GetType().Assembly;
                foreach (var name in assembly.GetManifestResourceNames())
                {
                    if (name.EndsWith(".modinfo.json"))
                    {
                        using var stream = assembly.GetManifestResourceStream(name)!;
                        using StreamReader sr = new StreamReader(stream);
                        string json = sr.ReadToEnd();
                        data = JsonSerializer.Deserialize<ModData>(json);
                    }
                }
                
                data ??= new ModData();
                data.ID = id;
                data.Name ??= info.Metadata.Name;
                data.Version = info.Metadata.Version.WithoutBuild().ToString();
                data.Dependencies = info.Dependencies
                    .Where(x => x.Flags == BepInDependency.DependencyFlags.HardDependency)
                    .Select(x => x.DependencyGUID)
                    .ToArray();

                if (TryGetIcon(assembly, data, out var icon))
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

    private static bool TryGetIcon(Assembly assembly, ModData data, out Sprite icon)
    {
        icon = null;
        if (data.IconResource.IsNullOrWhiteSpace())
            return false;

        try
        {
            icon = SpriteTools.LoadSpriteFromPath(data.IconResource, assembly, 100f);
        }
        catch (Exception e)
        {
            Logger<ModExplorerPlugin>.Error($"Unable to get icon for {data.ID}\n{e.ToString()}");
            return false;
        }

        return true;
    }
}