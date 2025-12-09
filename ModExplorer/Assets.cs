using ModExplorer.Utilities;
using Reactor.Utilities;
using UnityEngine;

namespace ModExplorer;

public static class Assets
{
    public static AssetBundle MainBundle { get; private set; }
    public static BundleAsset<GameObject> MenuPrefab => new(MainBundle, "ModExplorerCanvas"); 
    public static BundleAsset<GameObject> ModListElementPrefab => new(MainBundle, "ModListElement"); 
    
    public static void Load()
    {
        MainBundle = AssetBundleManager.Load("modexplorer");
    }
}