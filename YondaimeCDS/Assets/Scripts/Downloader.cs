using YondaimeFramework;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.IO;
using System;

public class Downloader : CustomBehaviour
{
    string path;
    string fileName = "some";
    
    void Start()
    {
        path = Application.persistentDataPath;
        StartCoroutine(GetAssetBundle());
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
        ContentBuilder builder = cube.GetComponent<ContentBuilder>();
    }


    void LogNames(AssetBundle bundle) 
    { 
        string[] names = bundle.GetAllAssetNames();
        foreach (string name in names)
            Debug.Log(name);
    }

    IEnumerator GetAssetBundle()
    {
        Debug.Log("Running");

        UnityWebRequest www = UnityWebRequest.Get("https://github.com/HokageMinato/YondaimeCDSContents/raw/main/content1");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            SaveAssetBundleToDisk(fileName, www.downloadHandler.data);

        }
        Debug.Log("End");
        yield return null;
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
}
