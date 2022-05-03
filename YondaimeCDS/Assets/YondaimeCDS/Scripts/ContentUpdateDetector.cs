using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace YondaimeCDS {

    public class ContentUpdateDetector 
    {
	    DownloaderConfig _downloaderConfig;
        private const string ANDROID_MANIFEST = "Android.manifest";
        
        public ContentUpdateDetector(DownloaderConfig config) 
        { 
            _downloaderConfig = config;
        }

        public async Task<bool> IsUpdateAvailable() 
        {
           byte[] serverManifest = await DownloadLatestManifest();
           byte[] localManifest = LoadLocalManifest();

            if (localManifest == null) 
            {
                SaveManifestData(serverManifest);
            }
            else
            {
                CompareAndSaveManifestData();
            }

           return false;
        }

        private void CompareAndSaveManifestData() 
        {
            Debug.Log("Comparing");
        }

        private void SaveManifestData(byte[] manifestData) 
        { 
            string fileName = Path.Combine(_downloaderConfig.StoragePath, ANDROID_MANIFEST);
            IOUtils.CreateMissingDirectory(fileName);
            IOUtils.SaveBytesToDisk(fileName, manifestData);
        }

        private async Task<byte[]> DownloadLatestManifest()
        {
           return  await new DownloadHandler(_downloaderConfig).DownloadContent(ANDROID_MANIFEST);
        }

        private byte[] LoadLocalManifest() 
        {
            string filePath = Path.Combine(_downloaderConfig.StoragePath,ANDROID_MANIFEST);
            IOUtils.CreateMissingDirectory(filePath);
            return IOUtils.LoadBytesFromDisk(filePath);

        }

        
    }

}