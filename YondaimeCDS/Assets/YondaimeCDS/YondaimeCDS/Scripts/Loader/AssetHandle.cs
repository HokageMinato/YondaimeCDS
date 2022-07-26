using UnityEngine;
using System;

namespace YondaimeCDS
{
    internal class AssetHandle : IEquatable<AssetHandle>
    {
        internal string BundleName { get; private set; }
        internal string AssetName { get; private set; }
        internal Action<float> OnOperationProgressChanged { get; private set; }
        internal bool IsDependencyBundle { get; private set; }



        internal AssetHandle(string bundleName, string assetName,Action<float> onOperationProgressChanged=null) 
        {
            BundleName = bundleName;
            AssetName = assetName;
            OnOperationProgressChanged = onOperationProgressChanged; 
            IsDependencyBundle = false;
        }
        
        internal AssetHandle(string bundleName, bool isDependencyBundle) 
        {
            BundleName = bundleName;
            IsDependencyBundle = isDependencyBundle;
        }

 

        internal void SetOperationProgress(float progressValue) 
        { 
            OnOperationProgressChanged?.Invoke(progressValue);
        }

        public bool Equals(AssetHandle other)
        {
            return other.GetHashCode() == GetHashCode();
        }
    }


    

}
