using UnityEngine;
using System;
using System.Collections.Generic;

namespace YondaimeCDS {

    [Serializable]
    public class ScriptManifest: ISerializationCallbackReceiver
    {
        [SerializeField] private BundleScriptHashTuple[] bundleWiseScriptHashes;
        [SerializeField] private List<string> allScriptHashList = new List<string>();
        
        private HashSet<string> allScriptHashes = new HashSet<string>();


      
        internal ScriptManifest()
        {
        }


        internal List<string> GetCompatibleBundleList(ScriptManifest serverScriptManifest)
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

        public void OnBeforeSerialize()
        {}

        public void OnAfterDeserialize()
        {
            allScriptHashes = new HashSet<string>(allScriptHashList);
        }


        #if UNITY_EDITOR
        public BundleScriptHashTuple[] BundleWiseScriptHashes
        {
            get
            {
                return bundleWiseScriptHashes;
            }
        }

        public void AddScriptHashes(List<string> scriptHashes)
        {
            for (int i = 0; i < scriptHashes.Count; i++)
            {
                string currentScriptHash = scriptHashes[i];
                if (!allScriptHashList.Contains(currentScriptHash))
                    allScriptHashList.Add(currentScriptHash);
            }
        }

        public ScriptManifest(int entryCount)
        {
            bundleWiseScriptHashes = new BundleScriptHashTuple[entryCount];
        }

        #endif

    }



    [Serializable]
    public class BundleScriptHashTuple
    {
        [SerializeField] internal string BundleName;
        [SerializeField] internal List<string> BundleSciptHashes;

        #if UNITY_EDITOR
        public BundleScriptHashTuple(string bundleName, List<string> bundleScriptHash)
        {
            BundleName = bundleName;
            BundleSciptHashes = bundleScriptHash;
        }
        #endif
    }

}