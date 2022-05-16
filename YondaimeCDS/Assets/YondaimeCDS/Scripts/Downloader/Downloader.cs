using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace YondaimeCDS
{
    public static class Downloader
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


        public static async Task<List<string>> CheckForContentUpdate()
        {
            return await new ContentUpdateDetector().GetUpdates();
        }

        public static async Task DownloadBundle(string assetName, Action<float> onProgressChanged = null)
        {
            if (IsDownloadAlreadyInProgress(assetName))
            {
                Debug.Log($"Download Task for {assetName} already in progress, ending thread");
                return;
            }
           
            if (!IsValidDownloadRequest(assetName))
            {
                Debug.Log("Bundle already Downloaded");
                return;
            }

            _activeDownloads.Add(assetName);
            bool downloadSuccess = await new BundleDownloadHandle().DownloadBundle(assetName, onProgressChanged);
            MarkBundleDownloaded(assetName, downloadSuccess);
        }

        private static void MarkBundleDownloaded(string assetName, bool downloadSuccess)
        {
            _activeDownloads.Remove(assetName);
            if (downloadSuccess)
                UpdateLocalManifest(assetName);
        }

        public static async Task<double> CalculateRemainingDownloadSize(string assetName)
        {
            return await new DownloadedDataTracker().GetRemainingDownloadSize(assetName);
        }
        
        private static void UpdateLocalManifest(string bundleName)
        {
            LocalAssetManifest.PendingUpdates.Remove(bundleName);
            ManifestTracker.UpdateAssetManifestDiskContents();
        }

        private static bool IsValidDownloadRequest(string bundleName)
        {
            return LocalAssetManifest.PendingUpdates.Contains(bundleName);
        }

        private static bool IsDownloadAlreadyInProgress(string bundleName) 
        {
            return _activeDownloads.Contains(bundleName);
        }
    }
}