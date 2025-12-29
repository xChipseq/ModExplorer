using System;
using System.Linq;
using System.Reflection;
using System.Text;
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
    
    public static string WrapLines(this string input, int maxCharsPerLine)
    {
        if (string.IsNullOrEmpty(input) || maxCharsPerLine <= 0)
            return input;

        var lines = input.Split('\n');
        var result = new StringBuilder(input.Length + 16);

        foreach (var line in lines)
        {
            int currentLength = 0;
            var words = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words)
            {
                int wordLength = word.Length;

                // New line needed
                if (currentLength > 0 && currentLength + 1 + wordLength > maxCharsPerLine)
                {
                    result.Append('\n');
                    currentLength = 0;
                }
                else if (currentLength > 0)
                {
                    result.Append(' ');
                    currentLength++;
                }

                result.Append(word);
                currentLength += wordLength;
            }

            result.Append('\n');
        }
        
        if (result.Length > 0)
            result.Length--;

        return result.ToString();
    }

}