using UnityEngine;
using UnityEngine.Networking;

public class CustomYTest : CustomYieldInstruction 
{
    private bool isDownloadRequestComplete = false;
    public byte[] data;

    public override bool keepWaiting
    {
        get
        {
            return isDownloadRequestComplete;
        }
    }


    public CustomYTest()
    {
        Download();
    }


    private void Download() 
    {
        using (UnityWebRequest downloadRequest = UnityWebRequest.Get("https://github.com/HokageMinato/YondaimeCDSContents/raw/main/testprefab"))
        {
            downloadRequest.SendWebRequest();

            while (!downloadRequest.isDone)
            {}

            Debug.Log(downloadRequest.result);

            if (downloadRequest.result == UnityWebRequest.Result.Success)
            {
                data = downloadRequest.downloadHandler.data;
            }

            else
                Debug.Log(downloadRequest.error);
        }
    }

}
