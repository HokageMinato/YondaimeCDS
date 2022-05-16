using UnityEngine;
using System;
using System.Collections.Generic;

namespace YondaimeCDS {

    [Serializable]
    public class ScriptManifest
    {
        [SerializeField] private BundleScriptHashTuple[] bundleWiseScriptHashes;
        [SerializeField] private List<string> allScriptHashes = new List<string>();
        //public string bit;

        #if UNITY_EDITOR
        public BundleScriptHashTuple[] BundleWiseScriptHashes => bundleWiseScriptHashes;
        
        #endif

        public ScriptManifest(int entryCount)
        {
            bundleWiseScriptHashes = new BundleScriptHashTuple[entryCount];
        }

        public ScriptManifest()
        {
        }


        public void AddScriptHashes(List<string> scriptHashes)
        {
            for (int i = 0; i < scriptHashes.Count; i++)
            {
                string currentScriptHash = scriptHashes[i];
                if (!allScriptHashes.Contains(currentScriptHash))
                    allScriptHashes.Add(currentScriptHash);
            }
        }


        public List<string> GetCompatibleBundleList(ScriptManifest serverScriptManifest)
        {
            List<string> compatibleBundles = ExtractBundleNames(serverScriptManifest);

            BundleScriptHashTuple[] serverManifestData = serverScriptManifest.bundleWiseScriptHashes;
            for (int i = 0; i < serverManifestData.Length; i++)
            {
                if (!IsBundleCompitable(serverManifestData[i]))
                    compatibleBundles.Remove(serverManifestData[i].BundleName);
            }

            return compatibleBundles;
        }



        private bool IsBundleCompitable(BundleScriptHashTuple serverBundleScriptData)
        {
            List<string> bundleScripts = serverBundleScriptData.BundleSciptHashes;

            for (int i = 0; i < bundleScripts.Count; i++)
            {
                if (!allScriptHashes.Contains(bundleScripts[i]))
                {
                    Debug.Log($"Script incompatibility detected for {serverBundleScriptData.BundleName}");
                    return false;
                }
            }

            return true;
        }

        private List<string> ExtractBundleNames(ScriptManifest serverScriptManifest)
        {
            List<string> compatibleBundles = new List<string>();
             BundleScriptHashTuple[] tuples = serverScriptManifest.bundleWiseScriptHashes;
            
            for (int i = 0; i < tuples.Length; i++) 
            {
                compatibleBundles.Add(tuples[i].BundleName);
            }

            return compatibleBundles;
        }

    }

    
    
    [Serializable]
    public class BundleScriptHashTuple
    {
        public string BundleName;
        public List<string> BundleSciptHashes;

        public BundleScriptHashTuple(string bundleName, List<string> bundleScriptHash)
        {
            BundleName = bundleName;
            BundleSciptHashes = bundleScriptHash;
        }
    }

}