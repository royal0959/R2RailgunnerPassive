using UnityEngine;
using System.IO;
using Plugin;

public static class Asset
{
    public static AssetBundle mainBundle;
    public const string bundleName = "passiveiconassetbundle";
    public const string assetBundleFolder = "AssetBundles";

    public static string AssetBundlePath
    {
        get
        {
            //return Path.Combine(Path.GetDirectoryName(Main.PInfo.Location), bundleName);
            return Path.Combine(Path.GetDirectoryName(Main.PInfo.Location), assetBundleFolder, bundleName);
        }
    }

    public static void Init()
    {
        mainBundle = AssetBundle.LoadFromFile(AssetBundlePath);
        Log.Info(mainBundle.Contains("textAtomStabilizerIcon"));
        Log.Info(mainBundle.Contains("textAtomStabilizerIcon.png"));
        Log.Info(mainBundle.Contains("Assets/textAtomStabilizerIcon.png"));
        Log.Info(mainBundle.Contains("Assets/textAtomStabilizerIcon"));


    }
}