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
            _IS_INITIALZIED = true;
        }

        #endregion

        #region CONTENT_TRACKING
        public static Task<IReadOnlyList<string>> CheckForContentUpdate()
        {
            if (!SystemInitializedCheck())
                return null;

            return Downloader.CheckForContentUpdate();
        }
        #endregion

        #region LOAD_HANDLES

        public static async Task<T> LoadAsset<T>(string bundleName, string assetName) where T : Object
        {
            if (!SystemInitializedCheck())
                return null;

            if (IsCatelogSetToAutoUpdate())
            {
                
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

        private static async Task DownloadUpdatedBundle(string bundleName)
        {
            IReadOnlyList<string> updates = await CheckForContentUpdate();
            if (updates != null && IsToBeUpdated(bundleName))
                await DownloadBundle(bundleName);


            bool IsToBeUpdated(string bundleName)
            {
                for (int i = 0; i < updates.Count; i++)
                    if (updates[i] == bundleName)
                        return true;

                return false;
            }
        }


        public static Task DownloadBundle(string bundleName, Action<float> OnProgressChanged=null)
        {
            if (!SystemInitializedCheck())
                return null;

            return Downloader.DownloadBundle(bundleName, OnProgressChanged);
        }

       

        public static double GetPendingDownloadSize(string assetName,SizeUnit sizeUnit = SizeUnit.Byte)
        {
            if (!SystemInitializedCheck())
                return -1;

            switch (sizeUnit)
            {
                case SizeUnit.MB:
                    return DownloadedDataTracker.GetPendingDownloadSizeInMB(assetName);
                    
                case SizeUnit.KB:
                    return DownloadedDataTracker.GetPendingDownloadSizeInKB(assetName);
                    
                default:
                    return DownloadedDataTracker.RequestBundleSize(assetName);
            }

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
            return _config.autoUpdateCatelog;
        }

        public static void Log(object data) 
        {
            Debug.Log($"bsys {data}");
        }
        #endregion

        #region INTERNAL_DECLARATIONS
        public enum SizeUnit
        {
            Byte,
            MB,
            KB
        }
        #endregion
    }
}