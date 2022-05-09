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
    string path;
    string fileName = "some";
    private static readonly HttpClient client = new HttpClient();
    public DownloaderConfig downloaderConfig;
    public Image progressbar;
    
    void Start()
    {
        

        path = Application.persistentDataPath;
        
        Debug.Log("st2");
        //Downloader.Initialize(downloaderConfig);
        StartCoroutine(Fetcher());
        //if (Downloader.CalculateRemainingDownloadSize("w2r2").Result > 0)
        //{
        //    Debug.Log("Trying download");
        //   Downloader.DownloadBundle("w2r2"); 
        //}
        //else 
        //{
        //    Debug.Log("Bundle already downloaded");
        //}
        
    }


    IEnumerator Fetcher() 
    {
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
        
        //Task download = Downloader.DownloadBundle("content",amount => { progressbar.fillAmount = amount; });
        //while(!download.IsCompleted)
        //    yield return null;

        //Task<List<string>> downloadTask2 = Downloader.CheckForContentUpdate();

        //while (!downloadTask2.IsCompleted)
        //{
        //    yield return null;
        //}

        //downloadList = downloadTask2.Result;
        //for (int i = 0; i < downloadList.Count; i++)
        //{
        //    Debug.Log(downloadList[i]);
        //}


    }

    
    

}
