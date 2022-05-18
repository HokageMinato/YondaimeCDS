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
    public Image progressbar;

    private void Start()
    {
        StartCoroutine(Fetcher());
    }


    private IEnumerator Fetcher() 
    { Debug.Log("st2");
        
        BundleSystem.Initialize();

        // Task<List<string>> downloadTask = BundleSystem.CheckForContentUpdate();
        //
        // while (!downloadTask.IsCompleted)
        // {
        //     yield return null;
        // }
        //
        // List<string> downloadList = downloadTask.Result;
        // for (int i = 0; i < downloadList.Count; i++)
        // {
        //     Debug.Log(downloadList[i]);
        // }
        // Debug.Log("Update check 1 done");
        //
        
         Task download = Downloader.DownloadBundle("content 3", DownloadProgress => { progressbar.fillAmount = DownloadProgress;});
        // Task download2 = Downloader.DownloadBundle("content 3",amount => { progressbar.fillAmount = amount; });
        //
        // while(!download.IsCompleted)
        //     yield return null;
        //
        // while(!download2.IsCompleted)
        //     yield return null;
        //
        //
        // Debug.Log("Donwload done");
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
        //
        // Debug.Log("Update check 2 done");

       Task<GameObject> loadTask = BundleSystem.LoadAsset<GameObject>("content", "Content");

       while (!loadTask.IsCompleted)
       {
           yield return null;
       }
       
       Task<GameObject> loadTask2 = BundleSystem.LoadAsset<GameObject>("content", "Content");
       
       while (!loadTask2.IsCompleted)
       {
           yield return null;
       }

       MonoInstantiate(loadTask.Result);
       yield return new WaitForSeconds(10);
       Debug.Log("Unloading");

       BundleSystem.UnloadBundle("content");

       Debug.Log("Unloaded");

    }

    
    

}
