using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace YondaimeCDS 
{
    internal class BundleDownloadHandle
    {

        private AssetHandle _assetHandle;

        internal async Task<bool> DownloadBundle(AssetHandle assetHandle)
        {
            
            _assetHandle = assetHandle;
            string bundleName = assetHandle.BundleName;
            string absoluteUrl = Path.Combine(BundleSystemConfig.REMOTE_URL, bundleName);
            string absoluteSavePath = Path.Combine(BundleSystemConfig.STORAGE_PATH, bundleName);
            bool downloadSuccess = await DownloadBundleContent(absoluteUrl,absoluteSavePath);
            
            _assetHandle = null;
            return downloadSuccess;
        }

        internal async Task<byte[]> DownloadContent(string contentName)
        {
            
            string url = Path.Combine(BundleSystemConfig.REMOTE_URL, contentName);

            using (UnityWebRequest downloadRequest = UnityWebRequest.Get(url))
            {
                downloadRequest.SendWebRequest();

                while (!downloadRequest.isDone)
                {
                    await Task.Delay(64);
                }


                if (downloadRequest.result == UnityWebRequest.Result.Success)
                {
                    return downloadRequest.downloadHandler.data;
                }

                else
                    BundleSystem.Log(downloadRequest.error);

                return null;
            }
        }

        private async Task<bool> DownloadBundleContent(string absoluteUrl, string absoluteSavePath)
        {
          
            using (UnityWebRequest downloadRequest = UnityWebRequest.Get(absoluteUrl))
            {
                downloadRequest.downloadHandler = new DownloadHandlerFile(absoluteSavePath);
                downloadRequest.SendWebRequest();

                while (!downloadRequest.isDone)
                {
                    _assetHandle.OnOperationProgressChanged?.Invoke(downloadRequest.downloadProgress);
                    await Task.Delay(64);
                }

                if (downloadRequest.result == UnityWebRequest.Result.Success)
                    return true;

                return false;
            }

           

        }



    }
}

