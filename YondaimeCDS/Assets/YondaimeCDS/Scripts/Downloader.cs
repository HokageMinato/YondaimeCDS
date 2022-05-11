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
        
        public static void Initialize(ScriptManifest localScriptManifest)
        {
            LocalScriptManifest = localScriptManifest;
            IsInitialzied = true;
            LoadLocalManifests();
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


        private static void LoadLocalManifests() 
        {
            LocalAssetManifest = IOUtils.LoadFromLocalDisk<SerializedAssetManifest>(Config.ASSET_MANIFEST);
            LocalHashManifest = IOUtils.LoadFromLocalDisk<HashManifest>(Config.MANIFEST_HASH);
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

    }


}
