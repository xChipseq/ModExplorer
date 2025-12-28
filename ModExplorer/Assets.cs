using ModExplorer.Utilities;
using Reactor.Utilities;
using UnityEngine;

namespace ModExplorer;

public static class Assets
{
    public static AssetBundle MainBundle { get; private set; }
    public static BundleAsset<GameObject> MenuPrefab => new(MainBundle, "ModExplorerCanvas"); 
    public static BundleAsset<GameObject> ModListElementPrefab => new(MainBundle, "ModListElement"); 
    
    public static BundleAsset<GameObject> SettingCategoryPrefab => new(MainBundle, "SettingCategory");
    public static BundleAsset<GameObject> BoolSettingPrefab => new(MainBundle, "BoolSetting");
    public static BundleAsset<GameObject> EnumSettingPrefab => new(MainBundle, "EnumSetting");
    public static BundleAsset<GameObject> FloatSettingPrefab => new(MainBundle, "FloatSetting");
    public static BundleAsset<GameObject> IntSettingPrefab => new(MainBundle, "IntSetting");
    public static BundleAsset<GameObject> StringSettingPrefab => new(MainBundle, "StringSetting");
    
    public static void Load()
    {
        MainBundle = AssetBundleManager.Load("modexplorer");
    }
}