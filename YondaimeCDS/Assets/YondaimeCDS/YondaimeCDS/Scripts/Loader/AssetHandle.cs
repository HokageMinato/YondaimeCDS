using UnityEngine;
using System;

namespace YondaimeCDS
{
    internal class AssetHandle 
    {
        internal string BundleName { get; private set; }
        internal string AssetName { get; private set; }
        internal Action<float> OnOperationProgressChanged { get; private set; }



        internal AssetHandle(string bundleName, string assetName,Action<float> onOperationProgressChanged=null) 
        {
            BundleName = bundleName;
            AssetName = assetName;
            OnOperationProgressChanged = onOperationProgressChanged; 
        }

        internal AssetHandle(string bundleName,Action<float> onOperationProgressChanged = null)
        {
            BundleName = bundleName;
            OnOperationProgressChanged= onOperationProgressChanged;
        }

        internal void SetLoadProgress(float progressValue) 
        { 
            OnOperationProgressChanged?.Invoke(progressValue);
        }


    }


    

}
