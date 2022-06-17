using System;
using System.IO;
using UnityEngine;
using UnityEngine.Build.Pipeline;
using UnityEditor;
using System.Collections.Generic;

namespace YondaimeCDS
{

    public class ManifestGenerator
    {

        public static void GenerateManifests(CompatibilityAssetBundleManifest buildManifest, string outputDirectory)
        {
            SerializedAssetManifest assetManifest = GenenrateAssetManifestFrom(buildManifest, outputDirectory);
            GenerateManifestHashFromContentsOf(assetManifest, outputDirectory);
            string serializedScriptManifest = GenerateScriptManifest(outputDirectory);
            string serializedAssetManifest = Utils.Serialize(assetManifest);
            WriteConfigToResources(GenerateBundleSystemConfigData(serializedScriptManifest,serializedAssetManifest));
            CopyLinkerXML(outputDirectory);
            DeleteExtraFilesAt(outputDirectory);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        private static void CopyLinkerXML(string outputDirectory)
        {
            string absoluteSourcePath = Path.Combine(outputDirectory, "link.xml");
            string absoluteTargetPath = Path.Combine("Assets","link.xml");
            string absoluteTargetP= Path.Combine("Assets","linkOld.xml");

            if (!File.Exists(absoluteSourcePath)) 
            { 
                File.Move(absoluteSourcePath,absoluteTargetPath);
                return;
            }

            File.Replace(absoluteSourcePath, absoluteTargetPath,absoluteTargetP);
            File.Delete(absoluteTargetP);
        }

        public static void WriteConfigToResources(BundleSystemConfig config)
        {
            string directory = Path.GetFullPath(".") + "/Assets/Resources";
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            string path = Path.Combine(directory, $"{Constants.SYSTEM_SETTINGS}.txt");
            File.WriteAllText(path, Utils.Serialize(config));
        }


        private static void GenerateManifestHashFromContentsOf(SerializedAssetManifest compatibilityAssetBundleManifest, string outputDirectory)
        {
            string manifestHash = ComputeHash(Utils.StringToBytes(Utils.Serialize(compatibilityAssetBundleManifest)));
            HashManifest hashManifest = new HashManifest();
            hashManifest.AssetHash = manifestHash;
            string serializedHashManifest = Utils.Serialize(hashManifest);
            byte[] hashManifestData = Utils.StringToBytes(serializedHashManifest);
            File.WriteAllBytes(Path.Combine(outputDirectory, Constants.MANIFEST_HASH), hashManifestData);
        }

        private static SerializedAssetManifest GenenrateAssetManifestFrom(CompatibilityAssetBundleManifest compatibilityBuildManifest, string outputDirectory)
        {
            SerializedAssetManifest manifest = Utils.Deserialize<SerializedAssetManifest>(Utils.Serialize(compatibilityBuildManifest));
            //manifest.SetLocalBundleList(localBundles);
            CalculateSizesOfBundle(manifest, outputDirectory);
            string serializedBundleManifest = Utils.Serialize(manifest);
            byte[] manifestData = Utils.StringToBytes(serializedBundleManifest);
            File.WriteAllBytes(Path.Combine(outputDirectory, Constants.ASSET_MANIFEST), manifestData);
            return manifest;
        }

        private static void CalculateSizesOfBundle(SerializedAssetManifest manifest,string targetDirectory) 
        {
            List<string> bundles = manifest.Keys;
            
            foreach (var item in bundles)
            {
               FileInfo bundleInfo = new FileInfo (Path.Combine(targetDirectory,item));
               manifest.SetBundleSizeOf(item,bundleInfo.Length);
               Debug.Log($"sizeData {item} -- {bundleInfo.Length}");
            }
            
        }

        private static string GenerateScriptManifest(string outputDirectory)
        {
            ScriptManifest scriptManifest = GetScriptManifest();
            string serializedScriptManifest = Utils.Serialize(scriptManifest);
            byte[] scriptManifestData = Utils.StringToBytes(serializedScriptManifest);
            File.WriteAllBytes(Path.Combine(outputDirectory, Constants.SCRIPT_MANIFEST), scriptManifestData);
            return serializedScriptManifest;
        }

        private static void DeleteExtraFilesAt(string outputDirectory)
        {
            File.Delete(Path.Combine(outputDirectory, $"{Path.GetFileName(outputDirectory)}.manifest"));
            File.Delete(Path.Combine(outputDirectory, "buildlogtep.json"));
        }

        private static BundleSystemConfig GenerateBundleSystemConfigData(string serializedScriptManifest, string serializedAssetManifest) 
        {
            return new BundleSystemConfig() { 
                serializedScriptManifest = serializedScriptManifest,
                //defaultSerializedAssetManifest = serializedAssetManifest
            };
        }

        private static ScriptManifest GetScriptManifest()
        {
            string[] assetBundles = AssetDatabase.GetAllAssetBundleNames();
            int totalAssetBundleCount = assetBundles.Length;

            ScriptManifest mf = new ScriptManifest(totalAssetBundleCount);
            BundleScriptHashTuple[] manifest = mf.BundleWiseScriptHashes;

            for (int i = 0; i < totalAssetBundleCount; i++)
            {
                string[] bundleAssetNames = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundles[i]);
                for (int j = 0; j < bundleAssetNames.Length; j++)
                {
                    string bundleName = assetBundles[i];
                    List<string> bundleScriptHashes = GenerateScriptHashesOfScriptDepencendies(GetScriptDependenciesOf(bundleAssetNames[j]));
                    manifest[i] = new BundleScriptHashTuple(bundleName, bundleScriptHashes);
                    mf.AddScriptHashes(bundleScriptHashes); 
                }
            }


            return mf;
        }

        static List<string> GetScriptDependenciesOf(string assetPath)
        {
            List<string> deps = new List<string>();
            string[] asset = AssetDatabase.GetDependencies(assetPath);

            for (int i = 0; i < asset.Length; i++)
            {
                if (asset[i].Contains(".cs"))
                {
                    deps.Add(asset[i]);
                }
            }

            return deps;

        }

        static List<string> GenerateScriptHashesOfScriptDepencendies(List<string> scriptDependencies)
        {
            List<string> scriptHashes = new List<string>();

            foreach (string scriptDependency in scriptDependencies)
            {
                scriptHashes.Add(ComputeHash(Utils.StringToBytes(GetContentOfScript(scriptDependency))));
            }

            return scriptHashes;
        }

        static string GetContentOfScript(string scriptLocation)
        {
            string scriptContent = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptLocation).text;
            scriptContent = scriptContent.Replace(" ", string.Empty);
            return scriptContent.Replace("\n", string.Empty);

        }


        private static string ComputeHash(byte[] data)
        {
            string hash = BitConverter.ToString(Utils.ToMD5(data));
            return hash.Replace("-", string.Empty);
        }

       

       
    }



}