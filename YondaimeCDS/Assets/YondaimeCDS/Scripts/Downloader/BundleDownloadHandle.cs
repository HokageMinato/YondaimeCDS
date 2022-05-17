using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace YondaimeCDS 
{

    public class BundleDownloadHandle
    {
        private Action<float> _onProgressChanged;

        public async Task<bool> DownloadBundle(string bundleName, Action<float> onProgressChanged = null)
        {
            _onProgressChanged = onProgressChanged;


            string absoluteSavePath = Path.Combine(Config.STORAGE_PATH, bundleName);
            bool downloadSuccess = await DownloadBundleContent(bundleName, absoluteSavePath);
            
            _onProgressChanged = null;
            return downloadSuccess;
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

        private async Task<bool> DownloadBundleContent(string bundleName, string absoluteSavePath)
        {
            string url = Path.Combine(Config.REMOTE_URL, bundleName);
            using (UnityWebRequest downloadRequest = UnityWebRequest.Get(url))
            {
                downloadRequest.downloadHandler = new DownloadHandlerFile(absoluteSavePath, true);

                downloadRequest.SendWebRequest();

                while (!downloadRequest.isDone)
                {
                    _onProgressChanged?.Invoke(downloadRequest.downloadProgress);
                    await Task.Yield();
                }


                if (downloadRequest.result == UnityWebRequest.Result.Success)
                    return true;

                Debug.Log(downloadRequest.error);

                return false;
            }

            //private void SaveAssetBundleToDisk(string fileName, byte[] assetBytes)
            //{
            //    IOUtils.SaveRawContentToLocalDisk(assetBytes, fileName);
            //}


        }
    }
}

