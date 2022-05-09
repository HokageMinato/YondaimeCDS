using UnityEngine;
using System;
using System.Collections.Generic;

namespace YondaimeCDS {

    [Serializable]
    public class ScriptManifest
    {
        public BundleScriptHashTuple[] bundleWiseScriptHashes;
        public List<string> allScriptHashes = new List<string>();
        public string bit;

        public ScriptManifest(int entryCount)
        {
            bundleWiseScriptHashes = new BundleScriptHashTuple[entryCount];
        }

        public ScriptManifest()
        { }


        public void AddScriptHashes(List<string> scriptHashes) 
        {
            for (int i = 0; i < scriptHashes.Count; i++)
            {
                string currentScriptHash = scriptHashes[i];
                if (!allScriptHashes.Contains(currentScriptHash))
                    allScriptHashes.Add(currentScriptHash);
            }
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

        public bool Equals(BundleScriptHashTuple tuple) 
        {
            bool allScriptsPresent = tuple.BundleSciptHashes.Count == BundleSciptHashes.Count;
            if(!allScriptsPresent)
                return false;

            List<string> otherScriptHashes = tuple.BundleSciptHashes;

            for (int i = 0; i < otherScriptHashes.Count; i++)
            {
                if (BundleSciptHashes[i] != otherScriptHashes[i])
                    return false;
            }

            return true;
        }
    }

}