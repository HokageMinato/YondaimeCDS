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

        public static string SERIZ_ASSET_MANIFEST;
        public static string SERIZ_SCRIPT_MANIFEST;
        public static string SERIZ_HASH_MANIFEST;


        public static void GenerateManifests(CompatibilityAssetBundleManifest buildManifest, string outputDirectory)
        {
            string serializedBundleManifest = IOUtils.Serialize(buildManifest);
            string manifestHash = ComputeHash(IOUtils.StringToBytes(serializedBundleManifest));

            SerializedAssetManifest manifest = JsonUtility.FromJson<SerializedAssetManifest>(serializedBundleManifest);
            serializedBundleManifest = IOUtils.Serialize(manifest);
            SERIZ_ASSET_MANIFEST = serializedBundleManifest;


            ScriptManifest scriptManifest = GetScriptManifest();
            string serializedScriptManifest = IOUtils.Serialize(scriptManifest);
            SERIZ_SCRIPT_MANIFEST = serializedScriptManifest;
            

            HashManifest hashManifest = new HashManifest();
            hashManifest.AssetHash = manifestHash;
            string serializedHashManifest = IOUtils.Serialize(hashManifest);
            SERIZ_HASH_MANIFEST = serializedHashManifest;

            byte[] manifestData = IOUtils.StringToBytes(serializedBundleManifest);
            byte[] scriptManifestData = IOUtils.StringToBytes(serializedScriptManifest);
            byte[] hashManifestData = IOUtils.StringToBytes(serializedHashManifest);


            File.WriteAllBytes(Path.Combine(outputDirectory, Constants.ASSET_MANIFEST), manifestData);
            File.WriteAllBytes(Path.Combine(outputDirectory, Constants.MANIFEST_HASH), hashManifestData);
            File.WriteAllBytes(Path.Combine(outputDirectory, Constants.SCRIPT_MANIFEST), scriptManifestData);

            WriteConfigToResources(new BundleSystemConfig() { serializedScriptManifest = serializedScriptManifest });

            File.Delete(Path.Combine(outputDirectory, $"{Path.GetDirectoryName(outputDirectory)}.manifest"));
            File.Delete(Path.Combine(outputDirectory, "buildlogtep.json"));

        }


        public static void WriteConfigToResources(BundleSystemConfig config)
        {
            string directory = Path.GetFullPath(".") + "/Assets/Resources";
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            string path = Path.Combine(directory, $"{Constants.SYSTEM_SETTINGS}.txt");
            File.WriteAllText(path,IOUtils.Serialize(config));
        }


        static ScriptManifest GetScriptManifest()
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


            //SaveInLocalScriptable
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
                scriptHashes.Add(ComputeHash(IOUtils.StringToBytes(GetContentOfScript(scriptDependency))));
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
            string hash = BitConverter.ToString(IOUtils.ToMD5(data));
            return hash.Replace("-", string.Empty);
        }

    }



}