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

    public static class Config
    {
        public static string STORAGE_PATH;
        public static string REMOTE_URL;

        public const string MANIFEST_HASH = "manifestHash";
        public const string MANIFEST = "manifest";
        public const string SCRIPT_MANIFEST = "scriptManifest";
    }

}