using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.Build.Pipeline;
using UnityEngine.Build.Pipeline;
using System.Linq;
using System.Text;
using UnityEditor.Build.Content;
using System.IO;
using System.Security.Cryptography;
using YondaimeCDS;
using System;

namespace AssetBundleBrowser.AssetBundleDataSource
{
    internal class AssetDatabaseABDataSource : ABDataSource
    {
        public static List<ABDataSource> CreateDataSources()
        {
            var op = new AssetDatabaseABDataSource();
            var retList = new List<ABDataSource>();
            retList.Add(op);
            return retList;
        }

        public string Name {
            get {
                return "Default";
            }
        }

        public string ProviderName {
            get {
                return "Built-in";
            }
        }

        public string[] GetAssetPathsFromAssetBundle (string assetBundleName) {
            return AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName);
        }

        public string GetAssetBundleName(string assetPath) {
            var importer = AssetImporter.GetAtPath(assetPath);
            if (importer == null) {
                return string.Empty;
            }
            var bundleName = importer.assetBundleName;
            if (importer.assetBundleVariant.Length > 0) {
                bundleName = bundleName + "." + importer.assetBundleVariant;
            }
            return bundleName;
        }

        public string GetImplicitAssetBundleName(string assetPath) {
            return AssetDatabase.GetImplicitAssetBundleName (assetPath);
        }

        public string[] GetAllAssetBundleNames() {
            return AssetDatabase.GetAllAssetBundleNames ();
        }

        public bool IsReadOnly() {
            return false;
        }

        public void SetAssetBundleNameAndVariant (string assetPath, string bundleName, string variantName) {
            AssetImporter.GetAtPath(assetPath).SetAssetBundleNameAndVariant(bundleName, variantName);
        }

        public void RemoveUnusedAssetBundleNames() {
            AssetDatabase.RemoveUnusedAssetBundleNames ();
        }

        public bool CanSpecifyBuildTarget { 
            get { return true; } 
        }
        public bool CanSpecifyBuildOutputDirectory { 
            get { return true; } 
        }

        public bool CanSpecifyBuildOptions { 
            get { return true; } 
        }

        public bool BuildAssetBundles (ABBuildInfo info) {
            if(info == null)
            {
                Debug.Log("Error in build");
                return false;
            }



            AssetBundleBuild[] bundles = ContentBuildInterface.GenerateAssetBundleBuilds();
            for (var i = 0; i < bundles.Length; i++)
            {
                string[] names = bundles[i].assetNames.Select(Path.GetFileNameWithoutExtension).ToArray();
                bundles[i].addressableNames = names;
            }

            CompatibilityAssetBundleManifest buildManifest = CompatibilityBuildPipeline.BuildAssetBundles(info.outputDirectory,bundles, info.options, info.buildTarget);
           
            if (buildManifest == null)
            {
                Debug.Log("Error in build");
                return false;
            }

            string manifestJSON = IOUtils.Serialize(buildManifest);
            string manifestHash = ComputeHash(IOUtils.StringToBytes(manifestJSON));
            manifestJSON+=(manifestHash);
           
            byte[] manifestData = IOUtils.StringToBytes(manifestJSON);
            byte[] hashData = IOUtils.StringToBytes(manifestHash);

            string outputDirectory = info.outputDirectory;

            File.WriteAllBytes(Path.Combine(outputDirectory, "manifest"),manifestData);
            File.WriteAllBytes(Path.Combine(outputDirectory, "manifestHash"),hashData);


            foreach (var assetBundleName in buildManifest.GetAllAssetBundles())
            {
                if (info.onBuild != null)
                {
                    info.onBuild(assetBundleName);
                }
            }
            return true;
        }

        private string ComputeHash(byte[] data) 
        {
            string hash = BitConverter.ToString(IOUtils.ToMD5(data));
            return hash.Replace("-",string.Empty);
        }

    }
}
