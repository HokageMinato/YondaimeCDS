using UnityEngine;
using System.Net;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace YondaimeCDS
{

    public static class Downloader
    {
        public static ContentUpdateDetector configUpdaterInstance;
        
        public static SerializedAssetManifest localAssetManifest;
        public static ScriptManifest localScriptManifest;
        

        public static void Initialize() 
        {
            localAssetManifest = IOUtils.Deserialize<SerializedAssetManifest>(IOUtils.LoadFromLocalDisk())
        }

        public static async Task<List<string>> CheckForContentUpdate() 
        {
           return await new ContentUpdateDetector().GetUpdates();
        }

        public static async Task DownloadBundle(string assetName, Action<float> OnProgressChanged=null) 
        { 
           await new DownloadHandler().DownloadBundle(assetName,OnProgressChanged);
        }
        
        public static async Task<double> CalculateRemainingDownloadSize(string assetName) 
        {
           return await new DownloadTracker().GetRemainingDownloadSize(assetName);
        }


       

    }


}
