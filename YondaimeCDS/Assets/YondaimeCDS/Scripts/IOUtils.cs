using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace YondaimeCDS {

    public class IOUtils : MonoBehaviour
    {
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

            public static string BytesToUnicode(byte[] content)
            {
                return Encoding.UTF8.GetString(content);
            }

        }

}

