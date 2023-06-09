using UnityEngine;
using System;

namespace YondaimeCDS  {

    [Serializable]
    public class HashManifest 
    {
        [SerializeField]internal string _assetHash;
       

        #if UNITY_EDITOR
        public string AssetHash { get => _assetHash; set => _assetHash = value; }
        #endif
    }

}