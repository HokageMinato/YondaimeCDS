using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace YondaimeCDS
{
    public class DownloadedDataTracker
    {
        private const string ResponseHeader = "Content-Length";

        public async Task<double> GetRemainingDownloadSize(string assetName)
        {
            double downloadedDataSize = IOUtils.GetOnDiskDataSize(assetName);
            double assetSize = await RequestBundleSize(Path.Combine(Config.REMOTE_URL, assetName));
            if (assetSize < 0)
                return -1;

            return downloadedDataSize - assetSize;
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
                    return (Convert.ToDouble(downloadRequest.GetResponseHeader(ResponseHeader)));
                }
                else
                {
                    return -1;
                }
            }
        }
    }
}