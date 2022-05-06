using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace YondaimeCDS {

    public class IOUtils
    {
        public const string MANIFEST = "manifest";
        public const string MANIFEST_HASH = "manifestHash";
        public const string SCRIPT_MANIFEST_HASH = "scriptManifest";

        public static void CreateMissingDirectory(string path)
        {
            if (IsDirectoryMissing(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static bool IsDirectoryMissing(string path)
        {
            string directoryName = Path.GetDirectoryName(path);
            return !Directory.Exists(directoryName);
        }

        public static void SaveBytesToDisk(string path, byte[] data)
        {
            try
            {
                File.WriteAllBytes(path, data);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }

        public static byte[] LoadBytesFromDisk(string path)
        {
            if (!File.Exists(path))
                return null;

            return File.ReadAllBytes(path);
        }

        public static string BytesToString(byte[] content)
        {
            return Encoding.UTF8.GetString(content);
        }

        public static byte[] StringToBytes(string data) 
        {
            return Encoding.UTF8.GetBytes(data);
        }

        public static T Deserialize<T>(string json)
        {
            return JsonUtility.FromJson<T>(json);
        }
        
        public static string Serialize<T>(T obj)
        {
            return JsonUtility.ToJson(obj);
        }


        public static byte[] ToMD5(byte[] data)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                return md5.ComputeHash(data);
            }

        }
    }
}

