using YondaimeFramework;
using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Net.Http;
using YondaimeCDS;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;

public class DownloadLoadTest : CustomBehaviour
{
    public DownloaderConfig downloaderConfig;
    public Image progressbar;

    private void Start()
    {
        

        
       
       
        StartCoroutine(Fetcher());
        
    }


    private IEnumerator Fetcher() 
    { Debug.Log("st2");
        
        Downloader.Initialize(downloaderConfig);
        
        Config.REMOTE_URL = downloaderConfig.remoteURL;
        Config.STORAGE_PATH = downloaderConfig.StoragePath;
        
        Task<List<string>> downloadTask = Downloader.CheckForContentUpdate();

        while (!downloadTask.IsCompleted)
        {
            yield return null;
        }

        List<string> downloadList = downloadTask.Result;
        for (int i = 0; i < downloadList.Count; i++)
        {
            Debug.Log(downloadList[i]);
        }
        Debug.Log("ed");
        //
        // Task download = Downloader.DownloadBundle("content",amount => { progressbar.fillAmount = amount; });
        // while(!download.IsCompleted)
        //     yield return null;
        //
        //
        // Task<List<string>> downloadTask2 = Downloader.CheckForContentUpdate();
        //
        // while (!downloadTask2.IsCompleted)
        // {
        //     yield return null;
        // }
        //
        // downloadList = downloadTask2.Result;
        // for (int i = 0; i < downloadList.Count; i++)
        // {
        //     Debug.Log(downloadList[i]);
        // }


    }

    
    

}
