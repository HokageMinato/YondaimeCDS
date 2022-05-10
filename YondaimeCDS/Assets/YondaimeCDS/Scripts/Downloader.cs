using UnityEngine;
using System.Net;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace YondaimeCDS
{

    public static class Downloader
    {
        public static ContentUpdateDetector ConfigUpdaterInstance;
        public static SerializedAssetManifest LocalAssetManifest;
        public static ScriptManifest LocalScriptManifest;
        public static HashManifest LocalHashManifest;
        
        
        public static void Initialize(ScriptManifest localScriptManifest)
        {
            LocalScriptManifest = localScriptManifest;
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


        public static void LoadLocalManifests() 
        {
            LocalAssetManifest = IOUtils.LoadFromLocalDisk<SerializedAssetManifest>(Config.ASSET_MANIFEST);
            LocalHashManifest = IOUtils.LoadFromLocalDisk<HashManifest>(Config.MANIFEST_HASH);
        }

        
         public static void SetStatusDownloaded(string bundleName)
         {
             Debug.Log(LocalAssetManifest == null);
             Debug.Log(LocalAssetManifest.PendingUpdates == null);
             LocalAssetManifest.PendingUpdates.Remove(bundleName);
             IOUtils.SaveToLocalDisk(LocalAssetManifest,Config.ASSET_MANIFEST);
         }

        public static void CreateHashManifestDiskContents(HashManifest serverHashManifest)
        {
            LocalHashManifest = serverHashManifest;
            IOUtils.SaveToLocalDisk(LocalHashManifest,Config.MANIFEST_HASH);
        }
        
        public static void WriteAssetManifestToDisk()
        {
            IOUtils.SaveToLocalDisk(LocalAssetManifest,Config.ASSET_MANIFEST);
        }

    }


}
