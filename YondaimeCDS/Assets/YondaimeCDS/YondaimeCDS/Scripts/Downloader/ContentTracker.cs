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
        private static IReadOnlyList<string> ServerAssetList;
        #endregion


        internal static async Task<IReadOnlyList<string>> GetServerAssetUpdatesList()
        {
            bool wasAwaitingForResult = _requestActive;
            while (_requestActive)
            {
                await Task.Delay(64);
            }

            if (wasAwaitingForResult)
            {
                BundleSystem.Log("UpdateRequestAlreadyActive, joining other thread result");
                return ServerAssetList;
            }

            _requestActive = true;
            ServerAssetList = await new ContentUpdateRequest().GetServerAssetUpdatesList();
            
            _requestActive = false;
            return ServerAssetList;
        }

        internal static IReadOnlyList<string> GetAssetList()
        {
            return AssetManifest.BundleNames;
        }

        

        internal static bool IsBundleDownloaded(AssetHandle assetHandle)
        {
            bool isMainBundleDownloaded = IsBundleDependencyDownloaded(assetHandle);
            if (!isMainBundleDownloaded)
                return false;

            string[] dependencies = AssetManifest.GetBundleDependencies(assetHandle);

            for (int i = 0; i < dependencies.Length; i++)
            {   
                AssetHandle handle = new AssetHandle(dependencies[i],true);
                if (!IsBundleDependencyDownloaded(handle))
                    return false;
            }
            return true;
        }

        

        internal static bool IsBundleAvailableInBuild(AssetHandle assetHandle) 
        {
            bool isMainBundleAvailable = IsBundleDependencyAvailableInBuild(assetHandle);
            if (!isMainBundleAvailable)
                return false;

            string[] dependencies = AssetManifest.GetBundleDependencies(assetHandle);

            for (int i = 0; i < dependencies.Length; i++)
            {
                AssetHandle handle = new AssetHandle(dependencies[i],true);
                if (!IsBundleDependencyAvailableInBuild(handle))
                    return false;
            }
            return true;
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

        internal static string[] GetAssetDependencies(AssetHandle assetHandle) 
        { 
            return AssetManifest.GetBundleDependencies(assetHandle);
        }

        private static bool IsBundleDependencyAvailableInBuild(AssetHandle assetHandle)
        {
            return Utils.GetSizeOfDataFromStreamedStorage(assetHandle.BundleName) > 0;
        }

        private static bool IsBundleDependencyDownloaded(AssetHandle assetHandle)
        {
            return Utils.GetSizeOfDataFromPersistantStorage(assetHandle.BundleName) == GetAssetSize(assetHandle) && !Utils.Contains(assetHandle.BundleName, ServerAssetList);
        }

    }
}
