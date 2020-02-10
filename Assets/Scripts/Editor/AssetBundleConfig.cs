using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AssetBundleConfig")]
public class AssetBundleConfig : ScriptableObject
{
    public string abExportPath = "Assets/StreamingAssets";
    public List<BundleCollection> bundles;
    
}

[Serializable]
public class BundleCollection
{
    public string bundleName;
    public List<GameObject> prefabs;
}
