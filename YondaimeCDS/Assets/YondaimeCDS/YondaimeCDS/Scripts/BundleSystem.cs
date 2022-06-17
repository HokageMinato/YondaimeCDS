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
        private static bool _IS_INITIALIZED;
        private static bool _initializationStarted = false;
        #endregion


        #region INITIALZER

        public static async Task Initialize()
        {
            if (_IS_INITIALIZED || _initializationStarted)
            {
                Debug.LogError("Either System Initialied or in progress,suspending request thread");
                return;
            }

            _initializationStarted = true;
            LoadSystemConfig();
            await InitializeManifestTracker();
            _IS_INITIALIZED = true;
        }

        private static void LoadSystemConfig()
        {
            _config = Utils.LoadFromResourcesTextAsset<BundleSystemConfig>(Constants.SYSTEM_SETTINGS);
            Debug.Log(_config.remoteURL + "iro");
        }

        private static async Task InitializeManifestTracker()
        {
            ManifestTracker.Initialize(_config);
            if (ManifestTracker.LocalAssetManifest == null)
                await ContentTracker.GetServerAssetUpdatesList();
        }

        public static async Task SetRemoteURL(string url)
        {
            await SystemInitWait();
            ValidURLCheck(url);
            _config.SetRemoteURL(url);
        }
        #endregion



        #region CONTENT_TRACKING
        public static async Task<IReadOnlyList<string>> CheckForContentUpdates()
        {
            await SystemInitWait();
            return await ContentTracker.GetServerAssetUpdatesList();
        }

        

        public static async Task<bool> IsValidAddress(string bundleName)
        {
            await SystemInitWait();
            AssetHandle assetHandle = new AssetHandle(bundleName);
            return ContentTracker.IsValidAddress(assetHandle);
        }

        public static async Task<double> GetAssetSize(string bundleName) 
        {
            await SystemInitWait();
            AssetHandle assetHandle = new AssetHandle(bundleName);
            if (!ContentTracker.IsValidAddress(assetHandle))
            {
                InValidAddressException(bundleName);
                return -1;
            }

            return ContentTracker.GetAssetSize(assetHandle);
        }
        #endregion



        #region LOAD_HANDLES
        public static async Task<T> LoadAsset<T>(string bundleName, string assetName, Action<float> onLoadProgressChanged = null) where T : Object
        {

            await SystemInitWait();
            AssetHandle loadHandle = new AssetHandle(bundleName,assetName,onLoadProgressChanged);
            
            if (!ContentTracker.IsValidAddress(loadHandle))
            {
                InValidAddressException(bundleName);
                return null;
            }

            bool isAssetToBeDownloaded = !await ContentTracker.IsAssetDownloaded(loadHandle) && 
                                                !ContentTracker.IsBundleAvailableInBuild(loadHandle) &&
                                                _config.autoUpdateCatelog; 
                                          
            if (isAssetToBeDownloaded)
                await Downloader.DownloadBundle(loadHandle);

            return await Loader.LoadAsset<T>(loadHandle);
        }

        public static async Task UnloadBundle(string bundleName)
        {
            await SystemInitWait();
            
            AssetHandle unloadHandle = new AssetHandle(bundleName);

            if (!ContentTracker.IsValidAddress(unloadHandle))
            {
                InValidAddressException(bundleName);
                return;
            }

            Loader.UnloadBundle(unloadHandle);
        }

        #endregion

        #region DOWNLOAD_HANDLES

        public static async Task<bool> DownloadBundle(string bundleName, Action<float> OnProgressChanged = null)
        {
            await SystemInitWait();

            AssetHandle downloadHandle = new AssetHandle(bundleName, OnProgressChanged);
            
            if (!ContentTracker.IsValidAddress(downloadHandle))
            {
                InValidAddressException(bundleName);
                return false;
            }

            return await Downloader.DownloadBundle(downloadHandle);
        }

        public static async Task<bool> IsDownloaded(string bundleName)
        {
            await SystemInitWait();
            AssetHandle downloadCheckHandle = new AssetHandle(bundleName);
            if (!ContentTracker.IsValidAddress(downloadCheckHandle))
            {
                InValidAddressException(bundleName);
                return false;
            }

            return await ContentTracker.IsAssetDownloaded(downloadCheckHandle);
        }
        #endregion


        #region SYSTEM_CHECKS
        
        private static void  InValidAddressException(string bundleName)
        {
            throw new Exception($"Invalid BundleKey Request {bundleName}");
        }

        private static void ValidURLCheck(string url) 
        {
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                throw new Exception("Invalid URL format");

        }

        private static async Task SystemInitWait() 
        {
            while (!_IS_INITIALIZED) 
            {
                await Task.Yield();
            }
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