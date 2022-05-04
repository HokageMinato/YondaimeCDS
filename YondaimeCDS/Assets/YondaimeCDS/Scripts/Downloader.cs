using UnityEngine;
using System.Net;
using System;
using System.Threading.Tasks;

namespace YondaimeCDS
{

    public static class Downloader
    {
        private static DownloaderConfig config;
        private const string CONTENTKEY = "$030452244";
        //public static bool LatestContentDownloaded { get { return PlayerPrefs.GetInt(CONTENTKEY, 0) == 1; } set { PlayerPrefs.SetInt(CONTENTKEY, (value) ? 1 : 0); } }

        public static void Initialize(DownloaderConfig downloaderConfig) 
        { 
            config = downloaderConfig;
            
        }

        public static async Task<bool> CheckForContentUpdate() 
        {
            return await new ContentUpdateDetector(config).IsUpdateAvailable();
        }

        public static async Task DownloadBundle(string assetName, Action<float> OnProgressChanged=null) 
        { 
           await new DownloadHandler(config).DownloadBundle(assetName,OnProgressChanged);
        }
        
        public static async Task<double> CalculateRemainingDownloadSize(string assetName) 
        {
           return await new DownloadTracker(config).GetRemainingDownloadSize(assetName);
        }

       

    }


}
