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
using UnityEngine.Networking;

public class DownloadLoadTest : CustomBehaviour
{
    public Image progressbar;

    private void Start()
    {
        
        StartCoroutine(Fetcher());
    }

    

    private IEnumerator Fetcher() 
    {
        
        BundleSystem.Initialize();
        Task<IReadOnlyList<string>> update = BundleSystem.CheckForContentUpdate();
        while (!update.IsCompleted) 
        {
            yield return null;  
        }

        while (update.Result.Count > 0)
        {
            Task downloadTask = BundleSystem.DownloadBundle(update.Result[update.Result.Count-1]);
            while (!downloadTask.IsCompleted)
            {
                yield return null;
            }
        }

        Debug.Log(update.Result.Count);
        foreach (var item in update.Result)
        {
            Debug.Log(item);
        }
        
    }

    
    

}
