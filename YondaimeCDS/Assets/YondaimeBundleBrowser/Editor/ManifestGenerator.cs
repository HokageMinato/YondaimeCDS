using System;
using System.IO;
using UnityEngine;
using UnityEngine.Build.Pipeline;
using YondaimeCDS;
using UnityEditor;

namespace AssetBundleBrowser.AssetBundleDataSource
{

    public class ManifestGenerator 
    {
        public static void GenerateAssetManifest(CompatibilityAssetBundleManifest buildManifest,string outputDirectory) 
        {
            string manifestJSON = IOUtils.Serialize(buildManifest);
            string manifestHash = ComputeHash(IOUtils.StringToBytes(manifestJSON));
            manifestJSON += (manifestHash);

            byte[] manifestData = IOUtils.StringToBytes(manifestJSON);
            byte[] hashData = IOUtils.StringToBytes(manifestHash);

            File.WriteAllBytes(Path.Combine(outputDirectory, IOUtils.MANIFEST), manifestData);
            File.WriteAllBytes(Path.Combine(outputDirectory, IOUtils.MANIFEST_HASH), hashData);
            File.Delete(Path.Combine(outputDirectory, "Android.manifest"));
        }

        public static void GenerateScriptManifest() 
        {
            string[] assetBundles = AssetDatabase.GetAllAssetBundleNames();
            foreach (var item in assetBundles)
            {
                string lst ="Assets : \n";
                string[] assets = AssetDatabase.GetAssetPathsFromAssetBundle(item);
                foreach (var itemm in assets)
                {
                    lst += itemm + "\n";
                }
                Debug.Log(lst);
            }

            
        }

        private static string ComputeHash(byte[] data)
        {
            string hash = BitConverter.ToString(IOUtils.ToMD5(data));
            return hash.Replace("-", string.Empty);
        }

    }

}