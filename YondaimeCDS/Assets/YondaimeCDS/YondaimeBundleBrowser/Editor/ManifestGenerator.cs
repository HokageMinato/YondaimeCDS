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
            string serializedBundleManifest = IOUtils.Serialize(buildManifest);
            string manifestHash = ComputeHash(IOUtils.StringToBytes(serializedBundleManifest));

            SerializedAssetManifest manifest = JsonUtility.FromJson<SerializedAssetManifest>(serializedBundleManifest);
            serializedBundleManifest = IOUtils.Serialize(manifest);


            ScriptManifest scriptManifest = GetScriptManifest();
            string serializedScriptManifest = IOUtils.Serialize(scriptManifest);
            BundleSystemConfig config = AssetDatabase.LoadAssetAtPath<BundleSystemConfig>("Assets/YondaimeCDS/YondaimeCDS/Data/BundleSystemConfig.asset");
            config.serializedScriptManifest = serializedScriptManifest;
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();



            HashManifest hashManifest = new HashManifest();
            hashManifest.AssetHash = manifestHash;
            string serializedHashManifest = IOUtils.Serialize(hashManifest);


            byte[] manifestData = IOUtils.StringToBytes(serializedBundleManifest);
            byte[] scriptManifestData = IOUtils.StringToBytes(serializedScriptManifest);
            byte[] hashManifestData = IOUtils.StringToBytes(serializedHashManifest);


            File.WriteAllBytes(Path.Combine(outputDirectory, Config.ASSET_MANIFEST), manifestData);
            File.WriteAllBytes(Path.Combine(outputDirectory, Config.MANIFEST_HASH), hashManifestData);
            File.WriteAllBytes(Path.Combine(outputDirectory, Config.SCRIPT_MANIFEST), scriptManifestData);
            //File.Delete(Path.Combine(outputDirectory, "Android.manifest"));
            File.Delete(Path.Combine(outputDirectory, "buildlogtep.json"));
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