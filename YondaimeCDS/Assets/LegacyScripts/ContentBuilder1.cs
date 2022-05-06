using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System;
using System.Security.Cryptography;


public class ContentBuilder1 : MonoBehaviour
{
    public SO so;

    private int somedeclarat;
    public const int ChangeLv = 4545;

    #if UNITY_EDITOR
    [ContextMenu("Test monoScript")]
    void Test()
    {
        //MonoScript monoScript = MonoScript.FromMonoBehaviour(builder);
        //string scripText = monoScript.text;
        //scripText = scripText.Replace(" ", string.Empty);
        //scripText = scripText.Replace("\n", string.Empty);

        //MD5 md5 = MD5.Create();
        //byte[] hashBytes =md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(scripText));

        //string hashValue = BitConverter.ToString(hashBytes);

        //Debug.Log($"{scripText}--{hashValue}");
       
        
    }
    #endif   

}
