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
        Downloader.Initialize(downloaderConfig);
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
        //Task downloadTask = Downloader.DownloadBundle("w2r2",amount => { progressbar.fillAmount = amount; });
        
        Task<bool> downloadTask = Downloader.CheckForContentUpdate();

        while (!downloadTask.IsCompleted)
        {
            yield return null;
        }


        Debug.Log($"ed2 {downloadTask.Result}");

    }

    void Downloading(int percantage) 
    { 
        Debug.Log(percantage);
    }

    void LoadAssetBundle()
    {
        string _fileName = Path.Combine(path, fileName);
        AssetBundle bundle = AssetBundle.LoadFromFile(_fileName);
        if (bundle == null)
            Debug.LogError("Failed to load");

        LogNames(bundle);

        GameObject cube = bundle.LoadAsset<GameObject>("Content");
        MonoInstantiate(cube);
    }


    void LogNames(AssetBundle bundle) 
    { 
        string[] names = bundle.GetAllAssetNames();
        foreach (string name in names)
            Debug.Log(name);
    }

    async void DownloadAssetBundle()
    {
        Debug.Log("Running");

        try
        {
            HttpResponseMessage response = await client.GetAsync("https://github.com/HokageMinato/YondaimeCDSContents/raw/main/content");
            byte[] content = await response.Content.ReadAsByteArrayAsync();
            SaveAssetBundleToDisk(fileName, content);
            response.Dispose();
        }
        catch (HttpRequestException e)
        {
            Debug.Log($"Errored {e.Message}");
        }


        Debug.Log("End");
        LoadAssetBundle();
    }

    private void SaveAssetBundleToDisk(string _fileName, byte[] assetBytes)
    {
        string fileName = Path.Combine(path, _fileName);
        Debug.LogError("Save asset bundle : " + fileName);
        Save(fileName, assetBytes);
    }
    private void Save(string path, byte[] data)
    {
        if (!Directory.Exists(Path.GetDirectoryName(path)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        }

        try
        {
            File.WriteAllBytes(path, data);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    [ContextMenu("Test")]
    public void ListAllNames() 
    {
       
    }

    public GameObject Go;

    

}
