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

        DownloaderConfig _downloaderConfig;
       
        public ContentUpdateDetector(DownloaderConfig config) 
        { 
            _downloaderConfig = config;
        }

        public async Task<bool> IsUpdateAvailable() 
        {
            byte[] serverManifestHash = await DownloadLatestManifestHash();
            string serverManifestHashStr = IOUtils.BytesToString(serverManifestHash);
            byte[] localManifestHash = LoadFromLocalDisk(MANIFEST_HASH);
            

            if (localManifestHash == null) 
            {
                SaveToLocalDisk(serverManifestHash,MANIFEST_HASH);
                await DownloadLatestManifest();
                return true;
            }

            if (AreHashesDifferent(localManifestHash, serverManifestHash))
            {
                SaveToLocalDisk(serverManifestHash,MANIFEST_HASH);
            }

            if (!IsCurrentManifestUpdated(serverManifestHashStr)) 
            {
                Debug.Log("Downloading Latest manifest");
                await DownloadLatestManifest();
                return true;
            }


            return false;
        }

       
        #region MANIFEST_HASH_MANAGEMENT
        private bool AreHashesDifferent(byte[] localManifestData, byte[] serverManifestData)
        {
            string localHash = IOUtils.BytesToString(localManifestData);
            string serverHash = IOUtils.BytesToString(serverManifestData);
            return localHash != serverHash;
        }
       
        private async Task<byte[]> DownloadLatestManifestHash()
        {
            return await new DownloadHandler(_downloaderConfig).DownloadContent(MANIFEST_HASH);
        }
        #endregion


        #region MANIFEST_MANAGEMENT
        private bool IsCurrentManifestUpdated(string serverHash) 
        { 
            string InManifestHash = LoadInManifestHash();
            if (InManifestHash == null)
                return false;

            return InManifestHash == serverHash;
        }

        private async Task DownloadLatestManifest()
        {
            byte [] manifest = await new DownloadHandler(_downloaderConfig).DownloadContent(MANIFEST);
            SaveToLocalDisk(manifest, MANIFEST);
        }

        private string LoadInManifestHash()
        {
            string path = Path.Combine(_downloaderConfig.StoragePath, MANIFEST);
            byte[] manifestData = IOUtils.LoadBytesFromDisk(path);
            
            if (manifestData == null)
                return null;

            string manifestContent = IOUtils.BytesToString(manifestData);
            return manifestContent.Substring(manifestContent.Length - 32, 32);
        }


        private void CompareAndSaveManifestData(byte[] localManifestData, byte[] serverManifestData)
        {
            CompatibilityAssetBundleManifest serverManifest = IOUtils.Deserialize<CompatibilityAssetBundleManifest>(IOUtils.BytesToString(serverManifestData));
            CompatibilityAssetBundleManifest localManifest = IOUtils.Deserialize<CompatibilityAssetBundleManifest>(IOUtils.BytesToString(localManifestData));
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