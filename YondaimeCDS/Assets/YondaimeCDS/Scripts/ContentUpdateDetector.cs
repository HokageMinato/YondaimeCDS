using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Build.Pipeline;

namespace YondaimeCDS {

    public class ContentUpdateDetector 
    {

        private static string MANIFEST_HASH { get { return Config.MANIFEST_HASH; } }
        private static string MANIFEST { get { return Config.MANIFEST; } }
        private static string SCRIPT_MANIFEST { get { return Config.SCRIPT_MANIFEST; } }

        byte[] serverManifestHashBuffer;

        string serverScriptManifestHash;
        string localScriptManifestHash;
        ScriptManifest serverScriptManifest;
        ScriptManifest localScriptManifest;
       

        
        string serverAssetManifestHash;
        string localAssetManifestHash;

        byte[] serverManifestBuffer;
        public SerializedAssetManifest localAssetManifest;
        SerializedAssetManifest serverAssetManifest;

        bool scriptUpdatesPresent = false;

        List<string> compatibleBundles = new List<string>();
       
       
        public async Task<List<string>> GetUpdates()
        {
            await ParseManifestHashes();
            
            if (localAssetManifestHash == null || localScriptManifestHash == null)
            {
                IOUtils.SaveToLocalDisk(serverManifestHashBuffer, MANIFEST_HASH);
                await DownloadAssetManifestFromServer();
                FillBundleListFromServerAssetManifest();
                CreateLocalAssetManifestFromServer();
                return localAssetManifest.PendingUpdates;
            }

            await FilterScriptInCompitableBundles();

            LoadLocalAssetManifest();

            if (NoCompitableBundlesPresent())
            {
                return GetPendingUpdatesFromLocalAssetManifest(); //if any
            }

            await PrepareAssetManifestUpdates();

            if (AssetUpdatesPresent())
            {
                SyncLocalAssetManifest();
                IOUtils.SaveToLocalDisk(serverManifestHashBuffer, MANIFEST_HASH);
            }

            return GetPendingUpdatesFromLocalAssetManifest();
        }

        private void CreateLocalAssetManifestFromServer()
        {
            localAssetManifest = serverAssetManifest;
            SyncLocalAssetManifest();
        }

        private async Task ParseManifestHashes()
        {
            serverManifestHashBuffer = await DownloadFromServer(MANIFEST_HASH);
            byte[] localManifestHashBuffer = IOUtils.LoadFromLocalDisk(MANIFEST_HASH);

            if (localManifestHashBuffer != null)
            {
                string localManifestHash = IOUtils.BytesToString(localManifestHashBuffer);
                localScriptManifestHash = localManifestHash.Substring(32, 32);
                localAssetManifestHash = localManifestHash.Substring(0, 32);
            }

            string serverManifestHash = IOUtils.BytesToString(serverManifestHashBuffer);
            serverScriptManifestHash = serverManifestHash.Substring(32, 32);
            serverAssetManifestHash = serverManifestHash.Substring(0, 32);
        }





        #region ASSET_MANIFEST_MANAGEMENT
        public void SetStatusDownloaded(string bundleName) 
        {
            if (localAssetManifest == null) {
                Debug.Log("Nu");
                return;
                    }
            localAssetManifest.PendingUpdates.Remove(bundleName);
            WriteLocalManifestToDisk();
        }

        private async Task PrepareAssetManifestUpdates()
        {
            if (localAssetManifestHash != serverAssetManifestHash)
            {
                await DownloadAssetManifestFromServer(); 

                if (!scriptUpdatesPresent)
                    FillBundleListFromServerAssetManifest();

                localAssetManifest.GenerateUpdateList(serverAssetManifest, ref compatibleBundles);
            }
        }


        private async Task DownloadAssetManifestFromServer()
        {
            serverManifestBuffer = await DownloadFromServer(MANIFEST);
            serverAssetManifest = IOUtils.Deserialize<SerializedAssetManifest>(IOUtils.BytesToString(serverManifestBuffer));
         
        }

        private void LoadLocalAssetManifest()
        {
            localAssetManifest = IOUtils.Deserialize<SerializedAssetManifest>(IOUtils.BytesToString(IOUtils.LoadFromLocalDisk(MANIFEST)));
        }

        private bool AssetUpdatesPresent() 
        { 
            return compatibleBundles.Count > 0;
        }

        private void SyncLocalAssetManifest() 
        {
            localAssetManifest.UpdateWithServerManifest(serverAssetManifest, ref compatibleBundles);
            WriteLocalManifestToDisk();
        }

        private void WriteLocalManifestToDisk() 
        {
           // IOUtils.SaveToLocalDisk(IOUtils.StringToBytes(IOUtils.PackManifest(localAssetManifest, serverAssetManifestHash)), MANIFEST);
        }

        private List<string> GetPendingUpdatesFromLocalAssetManifest() 
        {
            return localAssetManifest.PendingUpdates;
        }
        #endregion

        #region SCRIPT_MANIFEST_MANAGEMENT

        private async Task FilterScriptInCompitableBundles()
        {
            if (serverScriptManifestHash != localScriptManifestHash)
            {
                await LoadScriptManifests();
                FillBundleListFromServerScriptManifest();
                FilterIncompitableBundles();
                scriptUpdatesPresent = true;
                return;
            }

            scriptUpdatesPresent = false;
        }

        private async Task LoadScriptManifests() 
        {
            await LoadScriptManifestFromServer();
            LoadLocalScriptManifest();
        }

        private void FilterIncompitableBundles() 
        {
            BundleScriptHashTuple[] serverManifestData = serverScriptManifest.bundleWiseScriptHashes;
            for (int i = 0; i < serverManifestData.Length; i++)
            {
                if (!IsBundleCompitable(serverManifestData[i]))
                    compatibleBundles.Remove(serverManifestData[i].BundleName);
            }
        }

        private async Task LoadScriptManifestFromServer() 
        {
            string content = IOUtils.BytesToString(await DownloadFromServer(SCRIPT_MANIFEST));
            serverScriptManifest = IOUtils.Deserialize<ScriptManifest>(content);
        }

        private bool IsBundleCompitable(BundleScriptHashTuple bundleScriptData) 
        {
            List<string> localScripts = localScriptManifest.allScriptHashes;
            List<string> bundleScripts = bundleScriptData.BundleSciptHashes;

            for (int i = 0; i < bundleScripts.Count; i++) 
            {
                if (!localScripts.Contains(bundleScripts[i]))
                {
                    Debug.Log($"Script incompatibility detected for {bundleScriptData.BundleName}");
                    return false;
                }
            }

            return true;
        }

        private void LoadLocalScriptManifest() 
        {
            localScriptManifest = IOUtils.Deserialize<ScriptManifest>(IOUtils.BytesToString(IOUtils.LoadFromLocalDisk(SCRIPT_MANIFEST)));
        }

        private void FillBundleListFromServerScriptManifest() 
        {
            BundleScriptHashTuple[] tuples = serverScriptManifest.bundleWiseScriptHashes;

            for (int i = 0; i < tuples.Length; i++) 
            {
                compatibleBundles.Add(tuples[i].BundleName);
            }
        }

        private void FillBundleListFromServerAssetManifest() 
        {
            compatibleBundles.AddRange(serverAssetManifest.Keys);
        }
        #endregion

        #region IO_OPERATION

        private async Task<byte[]> DownloadFromServer(string manifestName)
        {
            return await new DownloadHandler().DownloadContent(manifestName);
            //Dont save, every first change detection will replace old with new and next run will mark incompitables compitable.
            //Write only if compatibility check passes.
        }

      

        private bool NoCompitableBundlesPresent()
        {
            return compatibleBundles.Count < 1 && scriptUpdatesPresent;
        }
        #endregion
    }

}