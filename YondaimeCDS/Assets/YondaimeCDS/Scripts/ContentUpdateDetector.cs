using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Build.Pipeline;

namespace YondaimeCDS {

    public class ContentUpdateDetector 
    {

        private static string MANIFEST_HASH { get { return IOUtils.MANIFEST_HASH; } }
        private static string MANIFEST { get { return IOUtils.MANIFEST; } }
        private static string SCRIPT_MANIFEST { get { return IOUtils.SCRIPT_MANIFEST_HASH; } }


        DownloaderConfig _downloaderConfig;


       
        public ContentUpdateDetector(DownloaderConfig config) 
        { 
            _downloaderConfig = config;
        }

        public async Task<bool> IsUpdateAvailable()
        {
            byte[] serverManifestHashBits = await DownloadFromServer(MANIFEST_HASH);
            byte[] localManifestHashBits = LoadFromLocalDisk(MANIFEST_HASH);

            if (localManifestHashBits == null)
            {
                SaveToLocalDisk(serverManifestHashBits, MANIFEST_HASH);
                SaveToLocalDisk(await DownloadFromServer(MANIFEST), MANIFEST);
                SaveToLocalDisk(await DownloadFromServer(SCRIPT_MANIFEST), SCRIPT_MANIFEST);
                return true;
            }

            string serverManifestHashData = IOUtils.BytesToString(serverManifestHashBits);
            string localManifestHashData = IOUtils.BytesToString(LoadFromLocalDisk(MANIFEST_HASH));


            string serverScriptManifestHash = serverManifestHashData.Substring(32, 31);
            string localScriptManifestHash = localManifestHashData.Substring(32, 31);


            List<string> compatibleBundleUpdates = new List<string>();

            if (serverScriptManifestHash != localScriptManifestHash)
                await GetScriptCompatibleDownloadableBundleList(compatibleBundleUpdates);

            if (NoCompitableBundles(compatibleBundleUpdates))
            {
                //Dont do anything simply return. dont even write to disk. otherwise next run will ruin comparision.
                return false;
            }

            string serverAssetManifestHash = serverManifestHashData.Substring(0, 31);
            string localAssetManifestHash = localManifestHashData.Substring(0, 31);

            if (serverAssetManifestHash != localAssetManifestHash)
            {
                byte[] serverMani

            }
        
            return false;
        }

        private static bool NoCompitableBundles(List<string> compatibleBundleUpdates)
        {
            return compatibleBundleUpdates.Count < 1;
        }








        #region MANIFEST_MANAGEMENT
        private async Task<byte[]> DownloadFromServer(string manifestName)
        {
            return await new DownloadHandler(_downloaderConfig).DownloadContent(manifestName);
            //Dont save, every first change detection will replace old with new and next run will mark incompitables compitable.
            //Write only if compatibility check passes.
        }

        

        #endregion

        #region SCRIPT_MANIFEST_MANAGEMENT

        private async Task GetScriptCompatibleDownloadableBundleList(List<string> compatibleBundleList) 
        {
            BundleScriptHashTuple[] serverManifestData = IOUtils.Deserialize<ScriptManifest>(await GetServerScriptManifest()).bundleWiseScriptHashes;
            
            for (int i = 0; i < serverManifestData.Length; i++)
            {
               if(IsBundleCompitable(serverManifestData[i]))
                    compatibleBundleList.Add(serverManifestData[i].BundleName);
            }
        }

        private async Task<string> GetServerScriptManifest() 
        {
            string content = IOUtils.BytesToString(await DownloadFromServer(SCRIPT_MANIFEST));
            return content.Substring(0, content.Length - 32);
        }

        private bool IsBundleCompitable(BundleScriptHashTuple bundleScriptData) 
        {
            List<string> localScripts = _downloaderConfig.localScriptManifest.scriptManifest.allScriptHashes;
            List<string> bundleScripts = bundleScriptData.BundleSciptHashes;

            for (int i = 0; i < bundleScripts.Count; i++) 
            { 
                if(!localScripts.Contains(bundleScripts[i]))
                    return false;
            }

            return true;
        }
        
        #endregion

        #region IO_OPERATION
        private void SaveToLocalDisk(byte[] contentData, string contentName)
        {
            string fileName = Path.Combine(_downloaderConfig.StoragePath, contentName);
            IOUtils.CreateMissingDirectory(fileName);
            IOUtils.SaveBytesToDisk(fileName, contentData);
        }

        private byte[] LoadFromLocalDisk(string contentName)
        {
            string filePath = Path.Combine(_downloaderConfig.StoragePath, contentName);
            return IOUtils.LoadBytesFromDisk(filePath);
        }
        #endregion
    }

}