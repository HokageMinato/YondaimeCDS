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
        
        public static void Initialize()
        {
            if (_IS_INITIALIZED || _initializationStarted)
            {
                Debug.LogError("Either System Initialied or in progress,suspending request thread");
                return;
            }

            _initializationStarted = true;
            LoadSystemConfig();
            InitializeManifestTracker();
            _IS_INITIALIZED = true;
        }

        private static void LoadSystemConfig()
        {
            _config = Utils.LoadFromResourcesTextAsset<BundleSystemConfig>(Constants.SYSTEM_SETTINGS);
            Debug.Log(_config.remoteURL + "iro");
        }

        private static void InitializeManifestTracker()
        {
            ManifestTracker.Initialize(_config);
            if (ManifestTracker.LocalAssetManifest == null)
                 ContentTracker.GetServerAssetUpdatesList();
        }

        public static void SetRemoteURL(string url)
        {
            SystemInitWait();
            ValidURLCheck(url);
            _config.SetRemoteURL(url);
        }
        #endregion



        #region CONTENT_TRACKING
        public static IReadOnlyList<string> CheckForContentUpdates()
        {
            SystemInitWait();
            return ContentTracker.GetServerAssetUpdatesList();
        }


        public static bool IsValidAddress(string bundleName)
        {
            SystemInitWait();
            AssetHandle assetHandle = new AssetHandle(bundleName);
            return ContentTracker.IsValidAddress(assetHandle);
        }

        public static double GetAssetSize(string bundleName) 
        {
            SystemInitWait();
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
        public static T LoadAsset<T>(string bundleName, string assetName, Action<float> onLoadProgressChanged = null) where T : Object
        {

            SystemInitWait();
            AssetHandle loadHandle = new AssetHandle(bundleName,assetName,onLoadProgressChanged);
            
            if (!ContentTracker.IsValidAddress(loadHandle))
            {
                InValidAddressException(bundleName);
                return null;
            }

            bool isAssetToBeDownloaded = !ContentTracker.IsAssetDownloaded(loadHandle) && 
                                                !ContentTracker.IsBundleAvailableInBuild(loadHandle) &&
                                                _config.autoUpdateCatelog;

            if (isAssetToBeDownloaded)
            {
                Downloader.DownloadBundle(loadHandle);
            }

            return Loader.LoadAsset<T>(loadHandle);
        }

        public static void UnloadBundle(string bundleName)
        {
            SystemInitWait();
            
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

        public static bool DownloadBundle(string bundleName, Action<float> OnProgressChanged = null)
        {
            SystemInitWait();
            AssetHandle downloadHandle = new AssetHandle(bundleName, OnProgressChanged);
            
            if (!ContentTracker.IsValidAddress(downloadHandle))
            {
                InValidAddressException(bundleName);
                return false;
            }

            return Downloader.DownloadBundle(downloadHandle);
        }

        public static bool IsDownloaded(string bundleName)
        {
            SystemInitWait();
            AssetHandle downloadCheckHandle = new AssetHandle(bundleName);
            if (!ContentTracker.IsValidAddress(downloadCheckHandle))
            {
                InValidAddressException(bundleName);
                return false;
            }

            return ContentTracker.IsAssetDownloaded(downloadCheckHandle);
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

        private static void SystemInitWait() 
        {
            while (!_IS_INITIALIZED) 
            {}
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