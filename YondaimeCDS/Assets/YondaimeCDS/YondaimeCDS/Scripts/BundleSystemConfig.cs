using System.IO;
using UnityEngine;
namespace YondaimeCDS
{

    public class BundleSystemConfig: ISerializationCallbackReceiver
    {
        public string remoteURL;
        public string localStorageFolderName = string.Empty;
        public string serializedScriptManifest;
        public bool autoUpdateCatelog;

        public static string STORAGE_PATH;
        public static string STREAM_PATH;
        public static string REMOTE_URL;

        private string GenerateStoragePath()
        {
           string relativePath = Application.persistentDataPath;

           if (localStorageFolderName != string.Empty)
              relativePath = Path.Combine(relativePath, localStorageFolderName);
           
            return relativePath;
        }


        public void OnBeforeSerialize()
        {}

        public void OnAfterDeserialize()
        {
            STORAGE_PATH = GenerateStoragePath();
            REMOTE_URL = remoteURL;
            STREAM_PATH = Application.streamingAssetsPath ;
        }
    }

    public static class Constants
    {
        public const string MANIFEST_HASH = "manifestHash";
        public const string ASSET_MANIFEST = "manifest";
        public const string SCRIPT_MANIFEST = "scriptManifest";
        public const string SYSTEM_SETTINGS = "systemsettings";
        
    }

}