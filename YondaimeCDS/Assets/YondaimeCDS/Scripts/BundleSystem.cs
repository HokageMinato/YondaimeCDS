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

        public static void Initialize(BundleSystemConfig config)
        {
            _config = config;
            Config.STORAGE_PATH = _config.StoragePath;
            Config.REMOTE_URL = _config.remoteURL;
            Config.STREAM_PATH = _config.StreamingPath;
            ManifestTracker.Initialize(_config.serializedScriptManifest);
            _IS_INITIALZIED = true;
        }

        #endregion


        #region LOAD_HANDLES

        public static async Task<T> LoadAsset<T>(string bundleName, string assetName) where T : Object
        {
            if (!SystemInitializedCheck())
                return null;

            if (IsCatelogSetToAutoUpdate())
            {
                List<string> updates = await CheckForContentUpdate();
                if (updates.Contains(bundleName)) 
                {
                    await DownloadBundle(bundleName);
                }
            }

            T loadedAsset = await Loader.LoadAsset<T>(bundleName, assetName);
            return loadedAsset;
        }

        public static void UnloadBundle(string bundleName)
        {
            if (!SystemInitializedCheck())
                return;

            Loader.UnloadBundle(bundleName);
        }

        #endregion

        #region DOWNLOAD_HANDLES

        public static Task DownloadBundle(string bundleName, Action<float> OnProgressChanged=null)
        {
            if (!SystemInitializedCheck())
                return null;

            return Downloader.DownloadBundle(bundleName, OnProgressChanged);
        }

        public static Task<List<string>> CheckForContentUpdate()
        {
            if (!SystemInitializedCheck())
                return null;


            return Downloader.CheckForContentUpdate();
        }

        public static Task<double> CalculateRemainingDownloadSize(string assetName)
        {
            if (!SystemInitializedCheck())
                return null;

            return Downloader.CalculateRemainingDownloadSize(assetName);
        }

        #endregion

        #region SYSTEM_CHECKS

        private static bool SystemInitializedCheck()
        {
            if (!_IS_INITIALZIED)
                Debug.LogError("Initialize System before performing any operations");

            return _IS_INITIALZIED;
        }

        private static bool IsCatelogSetToAutoUpdate() 
        {
            return ManifestTracker.CatelogSettings.autoUpdateCatelog;
        }

        public static void Log(object data) 
        {
            Debug.Log($"bsys {data}");
        }
        #endregion
    }
}