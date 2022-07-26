using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace YondaimeCDS
{
    internal static class Downloader
    {
        #region PRIVATE_VARS
        private static HashSet<string> _activeDownloads = new HashSet<string>();
        #endregion


        internal static async Task<bool> DownloadBundle(AssetHandle downloadHandle)
        {

            while (IsDownloadAlreadyInProgress(downloadHandle))
            { 
                await Task.Delay(64);
            }
           
            if (IsBundleAlreadyDownloaded(downloadHandle))
            {
                BundleSystem.Log($"Latest Bundle Available:{downloadHandle.BundleName}");
                return true;
            }

            _activeDownloads.Add(downloadHandle.BundleName);

            bool downloadSuccess = await new BundleDownloadHandle().DownloadBundle(downloadHandle);
            _activeDownloads.Remove(downloadHandle.BundleName);
            return downloadSuccess;
        }


       
        private  static bool IsBundleAlreadyDownloaded(AssetHandle downloadHandle)
        {
            return ContentTracker.IsBundleDownloaded(downloadHandle);
        }

        public static bool IsDownloadAlreadyInProgress(AssetHandle downloadHandle)
        {
            return _activeDownloads.Contains(downloadHandle.BundleName);
        }
    }
}