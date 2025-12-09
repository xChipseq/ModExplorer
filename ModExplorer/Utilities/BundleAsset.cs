using System;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace ModExplorer.Utilities;

public class BundleAsset<T>(AssetBundle bundle, string name) where T : UnityEngine.Object
{
    private T CachedAsset;
    
    public T Get()
    {
        if (CachedAsset != null)
        {
            return CachedAsset;
        }

        CachedAsset = bundle.LoadAsset<T>(name);

        if (CachedAsset == null)
        {
            throw new InvalidOperationException($"INVALID ASSET: {name}");
        }

        return CachedAsset;
    }
}