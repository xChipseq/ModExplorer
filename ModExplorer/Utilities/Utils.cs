using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using BepInEx.Unity.IL2CPP;

namespace ModExplorer.Utilities;

public static class Utils
{
    public static string SpaceOutString(string input)
    {
        // a lot of regex which I don't understand
        string spaced = Regex.Replace(input, "(?<=[a-z0-9])(?=[A-Z])|(?<=[A-Z])(?=[A-Z][a-z])", " ");
        return Regex.Replace(spaced, @"\s+", " ").Trim();
    }

    public static Assembly GetPluginAssembly(string id)
    {
        return IL2CPPChainloader.Instance.Plugins.FirstOrDefault(x => x.Key == id).Value.Instance?.GetType().Assembly;
    }
}