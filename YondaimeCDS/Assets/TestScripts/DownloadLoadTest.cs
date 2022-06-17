using YondaimeFramework;
using UnityEngine;
using System.Collections;
using YondaimeCDS;
using System.Threading.Tasks;
using UnityEngine.UI;
using System.Collections.Generic;



public class DownloadLoadTest : CustomBehaviour
{
    public Image progressbar;
    private HashSet<int> set= new HashSet<int>() { 1,2,3};

    [ContextMenu("Test")]
    private void Start()
    {
        StartCoroutine(MonakeyTestRoutine());
    }

    private IEnumerator MonakeyTestRoutine()
    {
        //MonkeyTest Routine

        string bundleName = "testprefabg";
        string assetName = "TestPrefabG";

        //string bundleName = "content";
        //string assetName = "Content";
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
        Task init = BundleSystem.Initialize();
        while (!init.IsCompleted)
            yield return null;
        Debug.Log("inie");
    }

    private IEnumerator IsDownloaded(string bundeName)
    {
        Task<bool> bundleUpdateTask = BundleSystem.IsDownloaded(bundeName);
        while (!bundleUpdateTask.IsCompleted)
            yield return null;

        Debug.Log($"{bundeName} is downloaded:{bundleUpdateTask.Result}");
    }

    private IEnumerator ValidAddressTest(string bundleName) 
    {
        Task<bool> validAddressTask = BundleSystem.IsValidAddress(bundleName);
        while (!validAddressTask.IsCompleted)
            yield return null;

        Debug.Log($"{bundleName} is valid add {validAddressTask.Result}");
    }

    private IEnumerator ListUpdates() 
    {
        
        Task<IReadOnlyList<string>> update = BundleSystem.CheckForContentUpdates();
        while (!update.IsCompleted)
        {
            yield return null;
        }

        string updateCount = $" updates: {update.Result.Count} \n";
        foreach (var item in update.Result)
            updateCount += $"{item} \n";
        
        Debug.Log(updateCount);
    }

    private IEnumerator DownloadBundle(string bundleName) 
    {
        Task<bool> downloadTask = BundleSystem.DownloadBundle(bundleName);
        while (!downloadTask.IsCompleted)
        {
            yield return null;
        }
        Debug.Log($"{bundleName}: Is download successful {downloadTask.Result}");
    }
    
    private IEnumerator GetBundleSize(string bundleName) 
    {
        Task<double> downloadTask = BundleSystem.GetAssetSize(bundleName);
        while (!downloadTask.IsCompleted)
        {
            yield return null;
        }
        Debug.Log($"{bundleName}: downloadSize {downloadTask.Result}");
    }

    private IEnumerator LoadBundle(string bundleName,string assetName) 
    {
        Task<GameObject> loadTask = BundleSystem.LoadAsset<GameObject>(bundleName,assetName, prog => { progressbar.fillAmount = prog; });

        while (!loadTask.IsCompleted)
            yield return null;

        GameObject loadedObject = loadTask.Result;

        #if UNITY_EDITOR
        InEditorShaderFix.FixShadersForEditor(loadedObject);
        #endif

        MonoInstantiate(loadedObject);
    }

}
