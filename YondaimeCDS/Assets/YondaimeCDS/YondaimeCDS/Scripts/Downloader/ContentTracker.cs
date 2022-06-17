using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace YondaimeCDS
{
    public static class ContentTracker
    {
        #region PRIVATE_VARS
        private static bool _requestActive = false;
        #endregion

        #region PRIVATE_PROPERTIES
        private static SerializedAssetManifest AssetManifest { get { return ManifestTracker.LocalAssetManifest; } }
        private static ScriptManifest ScriptManifest { get { return ManifestTracker.LocalScriptManifest; } }
        public static IReadOnlyList<string> ServerAssetList { get; private set; }
        #endregion


        internal static async Task<IReadOnlyList<string>> CheckForUpdates()
        {
            bool wasAwaitingForResult = _requestActive;
            while (_requestActive)
            {
             
                await Task.Yield();
            }

            if (wasAwaitingForResult)
            {
                Debug.LogError("UpdateRequestAlreadyActive, joining other thread result");
                return ServerAssetList;
            }

            _requestActive = true;
            ServerAssetList = await new ContentUpdateRequest().GetServerAssetList();
            
            _requestActive = false;
            return ServerAssetList;
        }

        internal static IReadOnlyList<string> GetAssetList()
        {
            if (AssetManifest == null)
            {
                Debug.LogError("No Internet Detected, Running AssetQuery in Offline Mode. All Bundles Keys may not be detected.");
                return ScriptManifest.GetAllBundleNames();
            }

            return AssetManifest.BundleNames;
        }

        internal async static Task<bool> IsAssetDownloaded(AssetHandle assetHandle) 
        {
            IReadOnlyList<string> updates = await CheckForUpdates();
            string bundleName = assetHandle.BundleName;
            return Utils.GetSizeOfDataFromPersistantStorage(bundleName) == GetAssetSize(assetHandle) && !Utils.Contains(bundleName,updates);
        }

        internal static bool IsBundleAvailableInBuild(AssetHandle assetHandle) 
        {
            return Utils.GetSizeOfDataFromStreamedStorage(assetHandle.BundleName) > 0;
        }

        internal static bool IsValidAddress(AssetHandle assetHandle)
        {
            return Utils.Contains(assetHandle.BundleName, GetAssetList());
        }

        internal static double GetAssetSize(AssetHandle assetHandle) 
        { 
            if(AssetManifest == null)
                return -1;

            return AssetManifest.GetBundleSize(assetHandle.BundleName);
        }
        
    }
}
