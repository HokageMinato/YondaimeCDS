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
                Log("Either System Initialied or in progress,suspending request thread");
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
            Log(_config.remoteURL + "iro");
        }

        private static async Task InitializeManifestTracker()
        {
            ManifestTracker.Initialize(_config);
            if (ManifestTracker.LocalAssetManifest == null)
                await ContentTracker.GetServerAssetUpdatesList();
        }

        public static async Task SetRemoteURL(string url)
        {
            while (!_IS_INITIALIZED) await Task.Delay(64);
            ValidURLCheck(url);
            _config.SetRemoteURL(url);
        }
        #endregion



        #region CONTENT_TRACKING
        public static async Task<IReadOnlyList<string>> CheckForContentUpdates()
        {
            while (!_IS_INITIALIZED) await Task.Delay(64);
            return await ContentTracker.GetServerAssetUpdatesList();
        }


        public static async Task<bool> IsValidAddress(string bundleName)
        {
            while (!_IS_INITIALIZED) await Task.Delay(64);
            AssetHandle assetHandle = new AssetHandle(bundleName,false);
            return ContentTracker.IsValidAddress(assetHandle);
        }

        public static async Task<double> GetAssetSize(string bundleName) 
        {
            while (!_IS_INITIALIZED) await Task.Delay(64);
            AssetHandle assetHandle = new AssetHandle(bundleName,false);
            if (!ContentTracker.IsValidAddress(assetHandle))
            {
                InValidAddressException(bundleName);
                return -1;
            }

            return ContentTracker.GetAssetSize(assetHandle);
        }
        #endregion



        #region LOAD_HANDLES
        public static async Task<T> LoadBundle<T>(string bundleName, string assetName, Action<float> onLoadProgressChanged = null) where T : Object
        {

            while (!_IS_INITIALIZED) await Task.Delay(64);
            AssetHandle loadHandle = new AssetHandle(bundleName,assetName,onLoadProgressChanged);
            
            if (!ContentTracker.IsValidAddress(loadHandle))
            {
                InValidAddressException(bundleName);
                return null;
            }

            await ContentTracker.GetServerAssetUpdatesList();

            bool doesAssetHavePendingDependencies = !ContentTracker.IsBundleDownloaded(loadHandle) && 
                                                    !ContentTracker.IsBundleAvailableInBuild(loadHandle) &&
                                                    _config.autoUpdateCatelog;

            if (doesAssetHavePendingDependencies)
            {
                await DownloadBundle(loadHandle.BundleName);
                await Task.Delay(1500);
            }

            await LoadBundleDependencies(loadHandle);

            return await Loader.LoadAsset<T>(loadHandle);
        }

        private static async Task LoadBundleDependencies(AssetHandle bundleHandle) 
        {

            AssetHandle[] dependencyHandles = GenerateAssetHandles(ContentTracker.GetAssetDependencies(bundleHandle));
            Task<bool>[] downloadTasks = new Task<bool>[dependencyHandles.Length];

            for (int i = 0; i < dependencyHandles.Length; i++)
            {
                Log($"Loading dependency {dependencyHandles[i].BundleName} of parentBundle {bundleHandle.BundleName}");
                downloadTasks[i] = Loader.TryLoadBundleFromDisk(dependencyHandles[i]);
            }


            for (int i = 0; i < downloadTasks.Length;)
            {
                Task<bool> downloadTask = downloadTasks[i];
                if (!downloadTask.IsCompleted)
                    await Task.Delay(64);
                else
                {
                    if (!downloadTask.Result)
                        LogError($"Loading failed For Dependency bundle {dependencyHandles[i].BundleName} , Result may have missing assets");

                    i++;
                }
            }


            AssetHandle[] GenerateAssetHandles(string[] addresses)
            {
                AssetHandle[] handles = new AssetHandle[addresses.Length];
                for (int i = 0; i < addresses.Length; i++)
                    handles[i] = new AssetHandle(addresses[i], true);

                return handles;
            }
            
        }


        public static async Task UnloadBundle(string bundleName)
        {
            while (!_IS_INITIALIZED) await Task.Delay(64);
            
            AssetHandle unloadHandle = new AssetHandle(bundleName,false);

            if (!ContentTracker.IsValidAddress(unloadHandle))
            {
                InValidAddressException(bundleName);
                return;
            }

            Loader.UnloadBundle(unloadHandle);

            AssetHandle[] handles = GenerateAssetHandles(ContentTracker.GetAssetDependencies(unloadHandle));
            for (int i = 0; i < handles.Length; i++)
            {
                Loader.UnloadBundle(handles[i]);
            }
           
            
            AssetHandle[] GenerateAssetHandles(string[] addresses)
            {
                AssetHandle[] handles = new AssetHandle[addresses.Length];
                for (int i = 0; i < addresses.Length; i++)
                    handles[i] = new AssetHandle(addresses[i], true);

                return handles;
            }
        }

        #endregion



        #region DOWNLOAD_HANDLES

        public static async Task<bool> DownloadBundle(string bundleName, Action<float> OnProgressChanged = null)
        {
            while (!_IS_INITIALIZED) await Task.Delay(64);

            AssetHandle downloadHandle = new AssetHandle(bundleName,string.Empty, OnProgressChanged);
            
            if (!ContentTracker.IsValidAddress(downloadHandle))
            {
                InValidAddressException(bundleName);
                return false;
            }

            await ContentTracker.GetServerAssetUpdatesList();
            await DownloadBundleDependencies(downloadHandle);
            return await Downloader.DownloadBundle(downloadHandle);

        }

        public static async Task<bool> IsDownloaded(string bundleName)
        {
            while (!_IS_INITIALIZED) await Task.Delay(64);
            
            AssetHandle downloadCheckHandle = new AssetHandle(bundleName,false);
           
            if (!ContentTracker.IsValidAddress(downloadCheckHandle))
            {
                InValidAddressException(bundleName);
                return false;
            }

            await ContentTracker.GetServerAssetUpdatesList();

            return ContentTracker.IsBundleDownloaded(downloadCheckHandle);
        }

        private static async Task DownloadBundleDependencies(AssetHandle bundleHandle) 
        {
            AssetHandle[] dependencyHandles = GenerateAssetHandles(ContentTracker.GetAssetDependencies(bundleHandle));
            Task<bool>[] downloadStatus = new Task<bool>[dependencyHandles.Length];

            for (int i = 0; i < dependencyHandles.Length; i++)
            {
                Log($"Downloading dependency {dependencyHandles[i].BundleName} of {bundleHandle.BundleName}");
                downloadStatus[i] = Downloader.DownloadBundle(dependencyHandles[i]);
            }

           
            for (int i = 0; i < downloadStatus.Length;)
            {
                Task<bool> downloadTask = downloadStatus[i];
                if (!downloadTask.IsCompleted)
                    await Task.Delay(64);
                else
                {
                    if (!downloadTask.Result)
                        LogError($"DownloadFailed For Dependency bundle {dependencyHandles[i].BundleName}");

                    i++;
                }
            }

            


            AssetHandle[] GenerateAssetHandles(string[] addresses)
            {
                AssetHandle[] handles = new AssetHandle[addresses.Length];
                for (int i = 0; i < addresses.Length; i++)
                    handles[i] = new AssetHandle(addresses[i], true);
                
                return handles; 
            }
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

        

        public static void Log(object data) 
        {
            #if UNITY_EDITOR
            Debug.Log($" <color=\"yellow\">{data} </color>");

            #elif UNITY_ANDROID
            Debug.Log($" <color=\"blue\">{data} </color>");
            
            #endif

        }

        public static void LogError(object data)
        {
            Debug.Log($" <color=\"orange\">{data} </color>");
        }
        #endregion


    }
}