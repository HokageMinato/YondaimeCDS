using System;
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

       
        internal static bool DownloadBundle(AssetHandle downloadHandle)
        {

            while (IsDownloadAlreadyInProgress(downloadHandle))
            {}

            if (IsBundleAlreadyDownloaded(downloadHandle))
            {
                Debug.Log($"Latest Bundle Available");
                return true;
            }

            _activeDownloads.Add(downloadHandle.BundleName);
            bool downloadSuccess = new BundleDownloadHandle().DownloadBundle(downloadHandle);
            MarkBundleDownloaded(downloadHandle, downloadSuccess);
            return downloadSuccess;
        }

        private static void MarkBundleDownloaded(AssetHandle downloadHandle, bool downloadSuccess)
        {
            string assetName = downloadHandle.BundleName;
            _activeDownloads.Remove(assetName);
        }

       
        private static bool IsBundleAlreadyDownloaded(AssetHandle downloadHandle)
        {
            return ContentTracker.IsAssetDownloaded(downloadHandle);
        }

        public static bool IsDownloadAlreadyInProgress(AssetHandle downloadHandle)
        {
            return _activeDownloads.Contains(downloadHandle.BundleName);
        }
    }
}