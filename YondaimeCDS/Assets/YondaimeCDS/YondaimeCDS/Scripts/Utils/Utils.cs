using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace YondaimeCDS {

    public class Utils
    {

        public static bool Contains(string value, IReadOnlyList<string> stt)
        {
            for (int i = 0; i < stt.Count; i++)
            {
                if (stt[i] == value)
                    return true;
            }

            return false;
        }

        public static float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            float fromAbs = value - fromMin;
            float fromMaxAbs = fromMax - fromMin;

            float normal = fromAbs / fromMaxAbs;

            float toMaxAbs = toMax - toMin;
            float toAbs = toMaxAbs * normal;

            float to = toAbs + toMin;

            return to;
        }

        public static T Deserialize<T>(string json)
        {
            return JsonUtility.FromJson<T>(json);
        }

        public static string Serialize<T>(T obj)
        {
            return JsonUtility.ToJson(obj);
        }


        public static void SaveObjectToLocalDisk<T>(T content, string contentName)
        {
            string fileName = Path.Combine(BundleSystemConfig.STORAGE_PATH, contentName);
            CreateMissingDirectory(fileName);
            SaveBytesToDisk(fileName, StringToBytes(Serialize(content)));
        }

        public static T LoadFromLocalDisk<T>(string contentName)
        {
            byte[] loadBuffer = LoadBytesFromDisk(Path.Combine(BundleSystemConfig.STORAGE_PATH, contentName));
            if (loadBuffer == null)
                return default;
            
            return Deserialize<T>(BytesToString(loadBuffer));
        }

        public static T LoadFromResourcesTextAsset<T>(string contentName)
        {
            return Deserialize<T>(Resources.Load<TextAsset>(contentName).text);
        }


        private static double GetOnDiskSize(string assetName, string relativePath) 
        {
            string path = Path.Combine(relativePath, assetName);
            FileInfo file = new FileInfo(path);
            if (file == null || !file.Exists)
                return 0;

            return file.Length;
        }

        public static double GetSizeOfDataFromPersistantStorage(string assetName) 
        {
            return GetOnDiskSize(assetName, BundleSystemConfig.STORAGE_PATH);
        }
        
        public static double GetSizeOfDataFromStreamedStorage(string assetName) 
        {
            return GetOnDiskSize(assetName, BundleSystemConfig.STREAM_PATH);
        }

        public static byte[] ToMD5(byte[] data)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                return md5.ComputeHash(data);
            }
        }
        public static string BytesToString(byte[] content)
        {
            return Encoding.UTF8.GetString(content);
        }

#region PRIVATES


        public static byte[] StringToBytes(string data)
        {
            return Encoding.UTF8.GetBytes(data);
        }
        private static void CreateMissingDirectory(string path)
        {
            if (IsDirectoryMissing(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private static bool IsDirectoryMissing(string path)
        {
            string directoryName = Path.GetDirectoryName(path);
            return !Directory.Exists(directoryName);
        }

        private static void SaveBytesToDisk(string path, byte[] data)
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

        private static byte[] LoadBytesFromDisk(string path)
        {
            if (!File.Exists(path))
                return null;

            return File.ReadAllBytes(path);
        }

       

#endregion

    }
}

