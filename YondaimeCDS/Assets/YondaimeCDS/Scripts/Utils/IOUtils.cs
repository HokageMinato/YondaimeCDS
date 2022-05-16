using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace YondaimeCDS {

    public class IOUtils
    {
        
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
            string fileName = Path.Combine(Config.STORAGE_PATH, contentName);
            CreateMissingDirectory(fileName);
            SaveBytesToDisk(fileName, StringToBytes(Serialize(content)));
        }

        //public static void SaveRawContentToLocalDisk(byte[] content, string contentName)
        //{
        //    string fileName = Path.Combine(Config.STORAGE_PATH, contentName);
        //    CreateMissingDirectory(fileName);
        //    SaveBytesToDisk(fileName, content);
        //}

        public static T LoadFromLocalDisk<T>(string contentName)
        {
            byte[] loadBuffer = LoadBytesFromDisk(Path.Combine(Config.STORAGE_PATH, contentName));
            if (loadBuffer == null)
                return default;
            
            return Deserialize<T>(BytesToString(loadBuffer));
        }

        public static double GetOnDiskDataSize(string assetName) 
        {
            string path = Path.Combine(Config.STORAGE_PATH, assetName);
            if (!File.Exists(path))
                return 0;

            FileInfo file = new FileInfo(path);
            return file.Length;
        }

        public static byte[] ToMD5(byte[] data)
        {
            // Use input string to calculate MD5 hash
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

