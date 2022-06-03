using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace YondaimeCDS
{
    public static class BundleSystem
    {
        #region PRIVATE_VARS
        private static BundleSystemConfig _config;
        private static bool _IS_INITIALZIED = false;
        #endregion


        #region INITIALZER
        public static void Initialize()
        {
            _config = IOUtils.LoadFromResourcesTextAsset<BundleSystemConfig>(Constants.SYSTEM_SETTINGS);
            ManifestTracker.Initialize(_config.serializedScriptManifest);
            ContentTracker.Initialize(_config.autoUpdateCatelog);
            _IS_INITIALZIED = true;
        }

        public static void SetRemoteURL(string url)
        {
            if (!SystemInitializedCheck())
                return;

            _config.SetRemoteURL(url);

        }
        #endregion



        #region CONTENT_TRACKING
        public static Task<IReadOnlyList<string>> GetUpdates()
        {
            if (!SystemInitializedCheck())
                return null;

            return new ContentUpdateDetector().GetUpdates(); 
        }

        public static Task<IReadOnlyList<string>> GetAssetList() 
        {
            if (!SystemInitializedCheck())
                return null;

            return ContentTracker.GetAssetList();
        }

        public static Task<bool> IsValidAddress(string bundleName) 
        {
            if (!SystemInitializedCheck())
                return null;

            return ContentTracker.IsValidAddress(bundleName);
        }
        #endregion



        #region LOAD_HANDLES
        public static async Task<T> LoadAsset<T>(string bundleName, string assetName,Action<float> onLoadProgressChanged = null) where T : Object
        {
            if (!SystemInitializedCheck())
                return null;

            float partialProg = 0;
            Action<float> loadOperationProgress = onLoadProgressChanged;
            if (IsCatelogSetToAutoUpdate()) 
            {
                await Downloader.DownloadBundle(new AssetHandle(bundleName, SetPartialDownloadProgress));
                loadOperationProgress = SetPartialLoadProgress;
            }

            T loadedAsset = await Loader.LoadAsset<T>(new AssetHandle(bundleName,assetName,loadOperationProgress));
            return loadedAsset;

            void SetPartialDownloadProgress(float downProg)
            {
                partialProg = (IOUtils.Remap(downProg, 0, 1, 0, 0.5f));
                onLoadProgressChanged(partialProg);
            }
            void SetPartialLoadProgress(float loadProg)
            {
                partialProg += IOUtils.Remap(loadProg, 0, 1, 0.5f, 1.0f);
                onLoadProgressChanged(partialProg);
            }
        }

        public static void UnloadBundle(string bundleName)
        {
            if (!SystemInitializedCheck())
                return;

           AssetHandle unloadHandle = new AssetHandle(bundleName);
           Loader.UnloadBundle(unloadHandle);
        }

        #endregion

        #region DOWNLOAD_HANDLES

        public static Task<bool> DownloadBundle(string bundleName, Action<float> OnProgressChanged=null)
        {
            if (!SystemInitializedCheck())
                return null;

            AssetHandle downloadHandle = new AssetHandle(bundleName,OnProgressChanged);
            return Downloader.DownloadBundle(downloadHandle);
        }

        public static Task<bool> IsDownloaded(string bundleName)
        {
            if (!SystemInitializedCheck())
                return null;

            return ContentTracker.IsBundleDownloaded(bundleName);

        }
        #endregion

        #region SYSTEM_CHECKS

        private static bool SystemInitializedCheck()
        {
            if (!_IS_INITIALZIED) 
                throw new Exception("Initialize System before performing any operations");

            return _IS_INITIALZIED;
        }

        private static bool IsCatelogSetToAutoUpdate() 
        {
            return _config.autoUpdateCatelog;
        }

        public static void Log(object data) 
        {
            Debug.Log($"bsys {data}");
        }
        #endregion

       
    }
}