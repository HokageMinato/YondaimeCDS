using UnityEngine;
using System;
using System.IO;
using UnityEngine.Networking;
using System.Collections;
using System.Threading.Tasks;

namespace YondaimeCDS 
{ 

    public class DownloadTracker
    {

        
        public async Task<double> GetRemainingDownloadSize(string assetName) 
        { 
            double downloadedDataSize =  GetOnDiskDataSize(assetName);
            double assetSize = await RequestBundleSize(Path.Combine(Config.REMOTE_URL, assetName));
            if (assetSize == -1)
                return assetSize;

            return downloadedDataSize - assetSize;
        }



        private double GetOnDiskDataSize(string assetName) 
        {
            string path = Path.Combine(Config.STORAGE_PATH, assetName);
            if (!File.Exists(path))
                return 0;

            FileInfo file = new FileInfo(path);
            return file.Length;
        }


        private async Task<double> RequestBundleSize(string url)
        {
            using (UnityWebRequest downloadRequest = UnityWebRequest.Head(url))
            {
                downloadRequest.SendWebRequest();

                while (!downloadRequest.isDone)
                {
                    await Task.Yield();
                }

                if (downloadRequest.result == UnityWebRequest.Result.Success)
                {
                    return (Convert.ToDouble(downloadRequest.GetResponseHeader("Content-Length")));
                }
                else
                {
                    return -1;
                }
            }
        }


    }


   

}