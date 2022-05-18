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
        string bundleName = "payload.zip";
        string baseurl = "https://github.com/HokageMinato/YondaimeCDSContents/raw/main/";
       
        string absoluteUrl = Path.Combine(baseurl, bundleName);
        string absoluteSavePath = Path.Combine(Application.persistentDataPath, bundleName);


        using (UnityWebRequest downloadRequest = UnityWebRequest.Get(absoluteUrl))
        {


            downloadRequest.downloadHandler = new DownloadHandlerFile(absoluteSavePath);
            downloadRequest.SendWebRequest();

            

            while (!downloadRequest.isDone) 
            { 
                yield return null;
                progressbar.fillAmount = downloadRequest.downloadProgress;
            }


            if (downloadRequest.result == UnityWebRequest.Result.Success)
            {
                
                Debug.Log("success");
            }
            else 
            { 
                 Debug.Log(downloadRequest.error);
            }

        }
    }

    
    

}
