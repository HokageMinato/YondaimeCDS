using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace YondaimeCDS
{
    internal static class Loader
    {
        private static Dictionary<string, AssetBundle> _LOADED_BUNDLES = new Dictionary<string, AssetBundle>();
        private static Dictionary<string, UnityEngine.Object> _LOADED_ASSETS = new Dictionary<string, UnityEngine.Object>();
        private static Dictionary<string, int> _COMMON_BUNDLE_REFCOUNT = new Dictionary<string, int>();


        internal static async Task<T> LoadAsset<T>(AssetHandle loadHandle) where T : UnityEngine.Object
        {

            if (!IsBundleLoaded(loadHandle))
            {
                await TryLoadBundleFromDisk(loadHandle);
            }

            if (!IsAssetLoaded(loadHandle))
            {
                TryLoadAssetFromLoadedBundle(loadHandle);
            }

            if (!IsAssetLoaded(loadHandle)) 
            {
                BundleSystem.Log($"Bundle load request failed for {loadHandle.BundleName}, Make sure bundle is downloaded before loading it"); 
                return null;
            }

            return (T)_LOADED_ASSETS[GenerateAssetKey(loadHandle)];
        }

        internal static async Task<bool> TryLoadBundleFromDisk(AssetHandle assetHandle)
        {
            string bundleKey = assetHandle.BundleName;

            if (_LOADED_BUNDLES.ContainsKey(bundleKey))
            {
                if (assetHandle.IsDependencyBundle)
                    IncreaseCommonBundleRefCount(assetHandle);
                
               
                return true;
            }

            
            AssetBundle bundle = await new BundleResourceRequest().LoadAssetBundle(assetHandle);
            if (bundle == null)
            {
                return false;
            }

            _LOADED_BUNDLES.Add(bundleKey, bundle);

            if (assetHandle.IsDependencyBundle)
            {
                IncreaseCommonBundleRefCount(assetHandle);
            }

            return true;
        }

        internal static void UnloadBundle(AssetHandle loadHandle)
        {
            string bundleKey = loadHandle.BundleName;

            if (!_LOADED_BUNDLES.ContainsKey(bundleKey))
                return;

            if (loadHandle.IsDependencyBundle)
            {
                DecreaseCommonBundleRefCount(loadHandle);
                if (ActiveReferencesPresent(bundleKey))
                    return;
            }
            

            AssetBundle bundle = _LOADED_BUNDLES[loadHandle.BundleName];
            RemoveAllAssetReferencesOfBundle(loadHandle, bundle);
            _LOADED_BUNDLES.Remove(loadHandle.BundleName);
            bundle.Unload(true);


            Debug.LogError($"Is Bundle loaded {loadHandle.BundleName} {IsBundleLoaded(loadHandle)}");
        }

        private static void DecreaseCommonBundleRefCount(AssetHandle assetHandle)
        {

            string bundleName = assetHandle.BundleName;
            if (_COMMON_BUNDLE_REFCOUNT.ContainsKey(bundleName))
            {
                _COMMON_BUNDLE_REFCOUNT[bundleName]--;
                BundleSystem.Log($"Decreasing ref count for {assetHandle.BundleName} present:{_COMMON_BUNDLE_REFCOUNT[bundleName]}");
            }

            if (_COMMON_BUNDLE_REFCOUNT[bundleName] <= 0)
            {
                _COMMON_BUNDLE_REFCOUNT.Remove(bundleName);
                BundleSystem.Log($"Removing ref count for {assetHandle.BundleName}");
            }
        }

        private static bool ActiveReferencesPresent(string bundleKey) 
        {
            return _COMMON_BUNDLE_REFCOUNT.ContainsKey(bundleKey);
        }

        private static void RemoveAllAssetReferencesOfBundle(AssetHandle loadHandle, AssetBundle bundle)
        {
            string[] assetNames = bundle.GetAllAssetNames();
            for (int i = 0; i < assetNames.Length; i++)
            {
                string key = loadHandle.BundleName + assetNames[i];
                if (_LOADED_ASSETS.ContainsKey(key))
                    _LOADED_ASSETS.Remove(key);
            }
        }

       

        private static void IncreaseCommonBundleRefCount(AssetHandle loadHandle)
        {
            string bundleKey = loadHandle.BundleName;

            if (_COMMON_BUNDLE_REFCOUNT.ContainsKey(bundleKey))  
                _COMMON_BUNDLE_REFCOUNT[bundleKey]++;
            else
                _COMMON_BUNDLE_REFCOUNT.Add(bundleKey, 1);

            BundleSystem.Log($"Added RefCount for {loadHandle.BundleName} present {_COMMON_BUNDLE_REFCOUNT[bundleKey]}");
        }

        private static void TryLoadAssetFromLoadedBundle(AssetHandle assetHandle) 
        {
            string assetKey = GenerateAssetKey(assetHandle);
            if (_LOADED_ASSETS.ContainsKey(assetKey))
                return;

            AssetBundle bundle = _LOADED_BUNDLES[assetHandle.BundleName];
            UnityEngine.Object asset = bundle.LoadAsset(assetHandle.AssetName);
            
            if (asset == null)
            {
                BundleSystem.Log($"No {assetHandle.AssetName} asset found in bundle {assetHandle.BundleName}");
                return;
            }

            _LOADED_ASSETS.Add(assetKey, asset);
        }

        


        private static bool IsBundleLoaded(AssetHandle handle) 
        { 
            return _LOADED_BUNDLES.ContainsKey(handle.BundleName);
        }

        private static bool IsAssetLoaded(AssetHandle handle) 
        { 
            return _LOADED_ASSETS.ContainsKey(GenerateAssetKey(handle));
        }

        private static string GenerateAssetKey(AssetHandle handle) 
        {
            return handle.BundleName + handle.AssetName;
        }

    }
}