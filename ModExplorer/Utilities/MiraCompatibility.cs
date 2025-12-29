using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace ModExplorer.Utilities;

public static class MiraCompatibility
{
    private const string MiraID = "mira.api";
    internal static bool IsMiraInstalled { get; private set; }
    
    private static readonly List<ConfigDefinition> moddedOptions = new();
    private static readonly Dictionary<string, List<string>> roleEntries = new();
    private static readonly Dictionary<string, PluginInfo> miraPlugins = new();

    internal static void ProcessPlugin(PluginInfo info, Assembly assembly)
    {
        var id = info.Metadata.GUID;
        if (id == MiraID)
        {
            IsMiraInstalled = true;
            InitMira(assembly);
            return;
        }
        
        // Check if the plugin is a Mira plugin
        var pluginType = info.Instance.GetType();
        if (pluginType.GetInterface("IMiraPlugin") != null)
        {
            // Search through all for custom roles and add their chance & num entries to the list
            var entries = new List<string>();
            foreach (var type in AccessTools.GetTypesFromAssembly(pluginType.Assembly))
            {
                if (type.GetInterface("ICustomRole") != null)
                {
                    entries.Add(NumConfigKey(type));
                    entries.Add(ChanceConfigKey(type));
                }
            }
            
            
            miraPlugins.Add(id, info);
            roleEntries.Add(id, entries);
        }
    }
    
    private static void InitMira(Assembly miraAssembly)
    {
        // Get all modded options and get their definitions
        var types = AccessTools.GetTypesFromAssembly(miraAssembly);
        var moddedOptionsManager = types.First(x => x.Name == "ModdedOptionsManager");
        var moddedOptionsInfo = AccessTools.Field(moddedOptionsManager, "ModdedOptions");
        var optionsValue = moddedOptionsInfo.GetValue(null);
        var options = optionsValue as IDictionary;
        foreach (DictionaryEntry entry in options!)
        {
            var valueType = entry.Value!.GetType();
            var property = AccessTools.Property(valueType, "ConfigDefinition");
            var definition = property.GetValue(entry.Value) as ConfigDefinition;
            moddedOptions.Add(definition);
        }
    }

    private static string NumConfigKey(Type type) => $"Num {type.FullName}";
    private static string ChanceConfigKey(Type type) => $"Chance {type.FullName}";

    public static bool IsMiraOption(string modId, ConfigDefinition definition)
    {
        if (!IsMiraInstalled)
        {
            return false;
        }
        
        return miraPlugins.TryGetValue(modId, out var plugin) && moddedOptions.Contains(definition);
    }
    
    public static bool IsMiraRoleOption(string modId, ConfigDefinition definition)
    {
        if (!IsMiraInstalled)
        {
            return false;
        }
        
        if (!miraPlugins.TryGetValue(modId, out var plugin))
        {
            return false;
        }

        var entries = roleEntries[modId];
        return definition.Section == "Roles" && entries.Any(x => x == definition.Key);
    }
}