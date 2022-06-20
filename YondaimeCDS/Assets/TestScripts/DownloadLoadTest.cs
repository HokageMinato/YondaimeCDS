using YondaimeFramework;
using UnityEngine;
using System.Collections;
using YondaimeCDS;
using System.Threading.Tasks;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Networking;

public class DownloadLoadTest : CustomBehaviour
{
    public Image progressbar;
    private HashSet<int> set= new HashSet<int>() { 1,2,3};

    [ContextMenu("Test")]
    private void Start()
    {
        StartCoroutine(MonakeyTestRoutine());
        //StartCoroutine(CustomYield());
        // CustomYTest yTest = new CustomYTest();
        //Debug.Log($"ending download sync {yTest.data.Length}");
    }

    int i = 0;

    

    private IEnumerator CustomYield() 
    {
        //Debug.Log("Starting download");
      
            yield return null;

        //Debug.Log("Starting download");
        //yield return yTest;
        //byte[] data = yTest.data;

        Debug.Log($"ending download ");
    }

    private IEnumerator MonakeyTestRoutine()
    {
        //MonkeyTest Routine

        string bundleName = "testprefabg";
        string assetName = "TestPrefabG";

        
        StartCoroutine(IsDownloaded(bundleName));
        StartCoroutine(ValidAddressTest(bundleName));
        StartCoroutine(ListUpdates());
        StartCoroutine(GetBundleSize(bundleName));
        StartCoroutine(DownloadBundle(bundleName));
        StartCoroutine(LoadBundle(bundleName, assetName));
        StartCoroutine(Initialize());

        yield return null;
    }



    private IEnumerator Initialize() 
    {
        Debug.Log("inis");
        BundleSystem.Initialize();
        yield return null;
        Debug.Log("inie");
    }

    private IEnumerator IsDownloaded(string bundeName)
    {
        //Task<bool> bundleUpdateTask = BundleSystem.IsDownloaded(bundeName);
        bool bundleUpdateTask = BundleSystem.IsDownloaded(bundeName);
        yield return null;
        Debug.Log($"{bundeName} is downloaded:{bundleUpdateTask}");
    }

    private IEnumerator ValidAddressTest(string bundleName) 
    {
        yield return null;
        bool validAddressTask = BundleSystem.IsValidAddress(bundleName);
        Debug.Log($"{bundleName} is valid add {validAddressTask}");
    }

    private IEnumerator ListUpdates() 
    {
        yield return null;
        IReadOnlyList<string> update = BundleSystem.CheckForContentUpdates();
        
        string updateCount = $" updates: {update.Count} \n";
        foreach (var item in update)
            updateCount += $"{item} \n";
        
        Debug.Log(updateCount);
    }

    private IEnumerator DownloadBundle(string bundleName) 
    {
        bool downloadTask = BundleSystem.DownloadBundle(bundleName);
        
        Debug.Log($"{bundleName}: Is download successful {downloadTask}");
        yield return null;
    }
    
    private IEnumerator GetBundleSize(string bundleName) 
    {
        double downloadTask = BundleSystem.GetAssetSize(bundleName);
        Debug.Log($"{bundleName}: downloadSize {downloadTask}");
        yield return null;
    }

    private IEnumerator LoadBundle(string bundleName,string assetName) 
    {
        GameObject loadedObject = BundleSystem.LoadAsset<GameObject>(bundleName,assetName, prog => { progressbar.fillAmount = prog; });

        #if UNITY_EDITOR
        InEditorShaderFix.FixShadersForEditor(loadedObject);
        #endif
        yield return null;
        MonoInstantiate(loadedObject);
    }








    


    private void Download()
    {
        using (UnityWebRequest downloadRequest = UnityWebRequest.Get("https://github.com/HokageMinato/YondaimeCDSContents/raw/main/testprefab"))
        {
            downloadRequest.SendWebRequest();

            while (!downloadRequest.isDone)
            { }

            Debug.Log($"Download Result {downloadRequest.result}");

            if (downloadRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log(downloadRequest.downloadHandler.data);
            }

            else
                Debug.Log(downloadRequest.error);
        }
    }
}
