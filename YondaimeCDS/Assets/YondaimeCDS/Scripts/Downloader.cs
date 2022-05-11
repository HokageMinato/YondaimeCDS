using UnityEngine;
using System.Net;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace YondaimeCDS
{

    public static class Downloader
    {
        public static SerializedAssetManifest LocalAssetManifest;
        public static ScriptManifest LocalScriptManifest;
        public static HashManifest LocalHashManifest;
        public static bool IsInitialzied;
        private static DownloaderConfig _config;

        public static void Initialize(DownloaderConfig config)
        {
            _config = config;
            InitializeConfig();
            LoadLocalManifests();
            IsInitialzied = true;
        }

        public static async Task<List<string>> CheckForContentUpdate() 
        {
           return await new ContentUpdateDetector().GetUpdates();
        }

        public static async Task DownloadBundle(string assetName, Action<float> onProgressChanged=null) 
        { 
           await new DownloadHandler().DownloadBundle(assetName,onProgressChanged);
        }
        
        public static async Task<double> CalculateRemainingDownloadSize(string assetName) 
        {
           return await new DownloadTracker().GetRemainingDownloadSize(assetName);
        }

        
        public static void UpdateHashManifestDiskContents(HashManifest serverHashManifest)
        {
            LocalHashManifest = serverHashManifest;
            IOUtils.SaveObjectToLocalDisk(LocalHashManifest,Config.MANIFEST_HASH);
        }
        
        public static void UpdateAssetManifestDiskContents()
        {
            IOUtils.SaveObjectToLocalDisk(LocalAssetManifest,Config.ASSET_MANIFEST);
        }

        
        private static void LoadLocalManifests()
        {
            LocalScriptManifest = IOUtils.Deserialize<ScriptManifest>(_config.serializedScriptManifest);
            LocalAssetManifest = IOUtils.LoadFromLocalDisk<SerializedAssetManifest>(Config.ASSET_MANIFEST);
            LocalHashManifest = IOUtils.LoadFromLocalDisk<HashManifest>(Config.MANIFEST_HASH);
        }

        private static void InitializeConfig()
        {
            Config.STORAGE_PATH = _config.StoragePath;
            Config.REMOTE_URL = _config.remoteURL;
        }

       

    }


}
