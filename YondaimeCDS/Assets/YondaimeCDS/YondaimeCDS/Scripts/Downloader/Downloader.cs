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

        #region PRIVATE_PROPERTIES
        private static SerializedAssetManifest LocalAssetManifest
        {
            get { return ManifestTracker.LocalAssetManifest; }
        }

        #endregion


        

        internal static async Task<bool> DownloadBundle(AssetHandle downloadHandle)
        {

            if (IsDownloadAlreadyInProgress(downloadHandle))
            {
                Debug.Log($"Download Task for {downloadHandle.BundleName} already in progress, ending thread");
                return false;
            }
           
            if (!IsValidDownloadRequest(downloadHandle))
            {
                Debug.Log("Bundle already Downloaded or Invalid key");
                return false;
            }

            _activeDownloads.Add(downloadHandle.BundleName);
            bool downloadSuccess = await new BundleDownloadHandle().DownloadBundle(downloadHandle);
            MarkBundleDownloaded(downloadHandle, downloadSuccess);
            return downloadSuccess;
        }

        private static void MarkBundleDownloaded(AssetHandle downloadHandle, bool downloadSuccess)
        {
            string assetName = downloadHandle.BundleName;
            _activeDownloads.Remove(assetName);
            if (downloadSuccess)
                UpdateLocalManifest(assetName);
        }

        
        private static void UpdateLocalManifest(string bundleName)
        {
            LocalAssetManifest.MarkBundleDownloaded(bundleName);
            ManifestTracker.UpdateAssetManifestDiskContents();
        }

        private static bool IsValidDownloadRequest(AssetHandle downloadHandle)
        {
            return LocalAssetManifest.IsBundleDownloadPending(downloadHandle.BundleName);
        }

        private static bool IsDownloadAlreadyInProgress(AssetHandle downloadHandle)
        {
            return _activeDownloads.Contains(downloadHandle.BundleName);
        }
    }
}