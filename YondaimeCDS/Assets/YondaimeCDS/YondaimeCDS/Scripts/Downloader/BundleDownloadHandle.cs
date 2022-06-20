using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace YondaimeCDS 
{
    internal class BundleDownloadHandle
    {

        private AssetHandle _assetHandle;

        internal bool DownloadBundle(AssetHandle assetHandle)
        {
            
            _assetHandle = assetHandle;
            string bundleName = assetHandle.BundleName;
            string absoluteUrl = Path.Combine(BundleSystemConfig.REMOTE_URL, bundleName);
            string absoluteSavePath = Path.Combine(BundleSystemConfig.STORAGE_PATH, bundleName);
            bool downloadSuccess = DownloadBundleContent(absoluteUrl,absoluteSavePath);
            
            _assetHandle = null;
            return downloadSuccess;
        }

        internal byte[] DownloadContent(string contentName)
        {
            
            string url = Path.Combine(BundleSystemConfig.REMOTE_URL, contentName);

            using (UnityWebRequest downloadRequest = UnityWebRequest.Get(url))
            {
                downloadRequest.SendWebRequest();

                while (!downloadRequest.isDone)
                {}


                if (downloadRequest.result == UnityWebRequest.Result.Success)
                {
                    return downloadRequest.downloadHandler.data;
                }

                else
                    Debug.Log(downloadRequest.error);

                return null;
            }
        }

        private bool DownloadBundleContent(string absoluteUrl, string absoluteSavePath)
        {
          
            using (UnityWebRequest downloadRequest = UnityWebRequest.Get(absoluteUrl))
            {
                downloadRequest.downloadHandler = new DownloadHandlerFile(absoluteSavePath);
                downloadRequest.SendWebRequest();

                while (!downloadRequest.isDone)
                {
                    _assetHandle.OnOperationProgressChanged?.Invoke(downloadRequest.downloadProgress);
                }

                if (downloadRequest.result == UnityWebRequest.Result.Success)
                    return true;

                return false;
            }

           

        }



    }
}

