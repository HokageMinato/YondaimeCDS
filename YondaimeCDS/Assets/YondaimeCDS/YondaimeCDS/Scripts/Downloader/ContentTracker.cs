using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace YondaimeCDS
{
    public static class ContentTracker
    {
        #region PRIVATE_VARS
        private static bool _autoUpdateCatelog = true;
        private static HashSet<string> _activeTrackRequests = new HashSet<string>();
        private static HashSet<string> _activeAssetRequests = new HashSet<string>();
        private static bool _requestActive = false;
        private static bool _assetFetchActive = false;
        #endregion


        internal static void Initialize(bool autoUpdateCatelog) 
        { 
            _autoUpdateCatelog = autoUpdateCatelog;
        }

        internal static async Task<IReadOnlyList<string>> CheckForUpdates() 
        {
            if (_requestActive)
            {
                Debug.LogError("Another thread already active, Suspending this");
                return null;
            }

            _requestActive = true;
            IReadOnlyList<string> updates = await new ContentUpdateDetector().GetUpdates();
            _requestActive = false;
            return updates;
        }

        internal static async Task<IReadOnlyList<string>> GetAssetList() 
        {
            if (!_autoUpdateCatelog)
                return ManifestTracker.LocalAssetManifest.BundleNames;

             if (_assetFetchActive) 
             {
                Debug.LogError("Another Asset request already in process, Suspending this request");
                return null;   
             }

            _assetFetchActive=true;
            await CheckForUpdates();
            IReadOnlyList<string> AssetList = ManifestTracker.LocalAssetManifest.BundleNames;
            _assetFetchActive = false;
            return AssetList;
        }

        internal static async Task<bool> IsBundleDownloaded(string bundleName) 
        {
            if (!_autoUpdateCatelog)
                return Contains(bundleName,ManifestTracker.LocalAssetManifest.PendingUpdates);

            if (_activeTrackRequests.Contains(bundleName))
            {
                Debug.LogError("Another Asset request already in process, Suspending this request");
                return false;
            }

            _activeTrackRequests.Add(bundleName);
            IReadOnlyList<string> updates = await CheckForUpdates();
            bool isDownloaded = !Contains(bundleName,updates);
            _activeTrackRequests.Remove(bundleName);
            return isDownloaded;
        }

        internal static async Task<bool> IsValidAddress(string bundleName) 
        {
            if (!_autoUpdateCatelog)
                return Contains(bundleName, ManifestTracker.LocalAssetManifest.BundleNames);

            if (_activeAssetRequests.Contains(bundleName))
            {
                Debug.LogError("Another Asset Valid request already in process, Suspending this request");
                return false;
            }
            
            _activeAssetRequests.Add(bundleName);
            await CheckForUpdates();
            bool isValid = Contains(bundleName,ManifestTracker.LocalAssetManifest.BundleNames);
            _activeAssetRequests.Remove(bundleName);
            return isValid;
        }

        private static bool Contains(string value,IReadOnlyList<string> stt) 
        {
            for (int i = 0; i < stt.Count; i++)
            {
                if(stt[i] == value)
                    return true;
            }

            return false;
        }
    }
}
