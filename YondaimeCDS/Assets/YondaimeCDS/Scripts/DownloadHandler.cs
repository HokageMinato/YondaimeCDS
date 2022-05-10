using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace YondaimeCDS 
{

    public class DownloadHandler 
    {
        private Action<float> _onProgressChanged;
        
        
        public async Task DownloadBundle(string bundleName,Action<float> OnProgressChanged=null)
        {
            _onProgressChanged = OnProgressChanged;

            byte[] bundleContent = await DownloadContent(bundleName);

            if (bundleContent != null)
            {
                SaveAssetBundleToDisk(bundleName, bundleContent);
                Downloader.SetStatusDownloaded(bundleName);
            }

            _onProgressChanged = null;
        }

        public async Task<byte[]> DownloadContent(string bundleName)
        {
            string url = Path.Combine(Config.REMOTE_URL, bundleName);

            using (UnityWebRequest downloadRequest = UnityWebRequest.Get(url))
            {
                downloadRequest.SendWebRequest();

                while (!downloadRequest.isDone)
                {
                    _onProgressChanged?.Invoke(downloadRequest.downloadProgress);
                    await Task.Yield();
                }


                if (downloadRequest.result == UnityWebRequest.Result.Success)
                {
                    return downloadRequest.downloadHandler.data;
                }

                else
                    Debug.Log(downloadRequest.error);

                return null;
            }
        }


        private void SaveAssetBundleToDisk(string fileName, byte[] assetBytes)
        {
            IOUtils.SaveToLocalDisk(assetBytes, fileName);
        }

        


      
        

    }

}