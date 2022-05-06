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
        public static void GenerateManifests(CompatibilityAssetBundleManifest buildManifest,string outputDirectory) 
        {
            string bundleManifest = IOUtils.Serialize(buildManifest);
            string manifestHash = ComputeHash(IOUtils.StringToBytes(bundleManifest));
            
            string scriptManifest = GetSerializableScriptManifest();
            string scriptManifestHash = ComputeHash(IOUtils.StringToBytes(scriptManifest));

            bundleManifest += manifestHash;
            scriptManifest += scriptManifestHash;

            byte[] manifestData = IOUtils.StringToBytes(bundleManifest);
            byte[] scriptManifestData= IOUtils.StringToBytes(scriptManifest);

            string combinedHash = manifestHash + scriptManifestHash;
            byte[] hashData = IOUtils.StringToBytes(combinedHash);


            File.WriteAllBytes(Path.Combine(outputDirectory, IOUtils.MANIFEST), manifestData);
            File.WriteAllBytes(Path.Combine(outputDirectory, IOUtils.MANIFEST_HASH), hashData);
            File.WriteAllBytes(Path.Combine(outputDirectory, IOUtils.SCRIPT_MANIFEST_HASH), scriptManifestData);
            File.Delete(Path.Combine(outputDirectory, "Android.manifest"));
        }


        static string GetSerializableScriptManifest()
        {
            string[] assetBundles = AssetDatabase.GetAllAssetBundleNames();
            int totalAssetBundleCount = assetBundles.Length;

            ScriptManifest mf = new ScriptManifest(totalAssetBundleCount);
            BundleScriptHashTuple[] manifest = mf.bundleWiseScriptHashes;

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

            LocalScriptManifest localManifest = AssetDatabase.LoadAssetAtPath<LocalScriptManifest>("Assets/YondaimeCDS/Data/LocalScriptManifest.asset");
            localManifest.scriptManifest = mf;
            EditorUtility.SetDirty(localManifest);

            return IOUtils.Serialize(mf);
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