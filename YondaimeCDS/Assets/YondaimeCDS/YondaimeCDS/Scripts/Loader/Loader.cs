using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace YondaimeCDS
{
    internal static class Loader
    {
        private static Dictionary<string, AssetBundle> _LOADED_BUNDLES = new Dictionary<string, AssetBundle>();
        private static Dictionary<string, UnityEngine.Object> _LOADED_ASSETS = new Dictionary<string, UnityEngine.Object>();


        internal static async Task<T> LoadAsset<T>(AssetHandle loadHandle) where T : UnityEngine.Object
        {
            T asset = TryLoadAssetFromAssetCache<T>(loadHandle);
            if(asset != null)
                return asset;

            asset = TryLoadAssetFromBundleCache<T>(loadHandle);
            if (asset != null)
                return asset;

            asset = await TryLoadAssetBundleFromDisk<T>(loadHandle);
            
            if (asset == null)
                Debug.LogError($"Bundle load request failed for {loadHandle.BundleName}, Make sure bundle is downloaded before loading it");

            return asset;
        }

        internal static void UnloadBundle(AssetHandle loadHandle)
        {
            if (!_LOADED_BUNDLES.ContainsKey(loadHandle.BundleName))
                return;

            AssetBundle bundle = _LOADED_BUNDLES[loadHandle.BundleName];
            RemoveAllAssetReferencesOfBundle(loadHandle, bundle);
            _LOADED_BUNDLES.Remove(loadHandle.BundleName);
            bundle.Unload(true);
        }

        

        private static T TryLoadAssetFromBundleCache<T>(AssetHandle loadHandle) where T: UnityEngine.Object
        {
            if (_LOADED_BUNDLES.ContainsKey(loadHandle.BundleName))
            {
                AssetBundle loadedBundle = _LOADED_BUNDLES[loadHandle.BundleName];
                return LoadAssetFromBundle<T>(loadedBundle, loadHandle);
            }
            return default;
        }

        private static T LoadAssetFromBundle<T>(AssetBundle bundle, AssetHandle loadHandle) where T : UnityEngine.Object
        {
            T loadedAsset = bundle.LoadAsset<T>(loadHandle.AssetName);
            if (loadedAsset == null)
                return default;

            string key = GenerateAssetKey(loadHandle);
            _LOADED_ASSETS.Add(key, loadedAsset);
            return loadedAsset;
        }

        private static T TryLoadAssetFromAssetCache<T>(AssetHandle assetHandle) where T : UnityEngine.Object
        {
            string key = GenerateAssetKey(assetHandle);
            if (_LOADED_ASSETS.ContainsKey(key))
                return (T)_LOADED_ASSETS[key];
            
            return default;
        }


        private static async Task<T> TryLoadAssetBundleFromDisk<T>(AssetHandle assetHandle) where T : UnityEngine.Object 
        {
            AssetBundle bundle = await new BundleResourceRequest().LoadAssetBundle(assetHandle);
            
            if (bundle == null)
                return default;
            
            _LOADED_BUNDLES.Add(assetHandle.BundleName, bundle);
            return LoadAssetFromBundle<T>(bundle, assetHandle);
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

        private static string GenerateAssetKey(AssetHandle loadHandle)
        {
            return loadHandle.BundleName + loadHandle.AssetName;
        }

      

    }
}