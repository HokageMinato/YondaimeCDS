using System.IO;
using UnityEngine;

namespace YondaimeCDS
{

    [CreateAssetMenu(fileName = "DownloaderConfig",menuName = "YondaimeCDS/DownloaderConfig")]
    public class DownloaderConfig : ScriptableObject
    {
        public string remoteURL;
        public string localStorageFolderName = string.Empty;
        public string StoragePath
        {
            get
            {
                string relativePath = Application.persistentDataPath;

                if (localStorageFolderName != string.Empty)
                    relativePath = Path.Combine(relativePath, localStorageFolderName);

                return relativePath;
            }
        }

        

    }

}