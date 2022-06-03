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
        StartCoroutine(Fetcher());
    }

    

    private IEnumerator Fetcher() 
    {
        BundleSystem.Initialize();

        //Bundle Update Detect CHeck
        string bundleName = "content (1)";
        //Task<bool> bundleUpdateTask = BundleSystem.IsDownloaded(bundeName);

        //while (!bundleUpdateTask.IsCompleted) 
        //    yield return null;

        //Debug.Log($"{bundeName} {bundleUpdateTask.Result}");



        //Valid AddressTest
        Task<bool> validAddressTask = BundleSystem.IsValidAddress("sds");
        while (!validAddressTask.IsCompleted) 
            yield return null;

        Debug.Log($"sds is valid add {validAddressTask.Result}");



        //Content Update Test
        //Task<IReadOnlyList<string>> update = BundleSystem.GetUpdates();
        //while (!update.IsCompleted)
        //{
        //    yield return null;
        //}

        //Debug.Log(update.Result.Count);
        //foreach (var item in update.Result)
        //{
        //    Debug.Log(item);
        //}




        //All Download Test
        //while (update.Result.Count > 0)
        //{
        //    Task<bool> downloadTask = BundleSystem.DownloadBundle(update.Result[update.Result.Count - 1]);
        //    while (!downloadTask.IsCompleted)
        //    {
        //        yield return null;
        //    }
        //    Debug.Log($"Is download successful {downloadTask.Result}");
        //}

        //Debug.Log(update.Result.Count);
        //foreach (var item in update.Result)
        //{
        //    Debug.Log(item);
        //}




        ////Load Test
        //string status = string.Empty;
        //Task<GameObject> loadTask = BundleSystem.LoadAsset<GameObject>("content (1)","Content (1)", prog => { progressbar.fillAmount = prog; });

        //while (!loadTask.IsCompleted)
        //    yield return null;

        //GameObject loadedObject = loadTask.Result;

        //#if UNITY_EDITOR
        //InEditorShaderFix.FixShadersForEditor(loadedObject);
        //#endif

        //MonoInstantiate(loadedObject);

    }




}
