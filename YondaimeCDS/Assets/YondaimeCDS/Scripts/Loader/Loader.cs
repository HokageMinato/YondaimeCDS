
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace YondaimeCDS {

    public static class Loader 
    {
        private static Dictionary<string, AssetBundle> _LOADED_BUNDLES = new Dictionary<string, AssetBundle>();
        private static Dictionary<string, Object> _LOADED_ASSETS = new Dictionary<string, Object>();

        public static async Task<T> LoadAsset<T>(string bundleName, string assetName) where T : UnityEngine.Object
        {
            if (IsBundleLoaded(bundleName))
            {
                string key = GenerateAssetKey(bundleName,assetName);
                if(IsAssetLoaded(key))
                   return (T)_LOADED_ASSETS[key];
                   
                AssetBundle loadedBundle = _LOADED_BUNDLES[bundleName];
                return LoadAssetFromBundle<T>(loadedBundle, bundleName, assetName);
            }
            
            AssetBundle bundle = await LoadBundle<T>(bundleName);
            T loadedAsset = LoadAssetFromBundle<T>(bundle,assetName,bundleName);

            return loadedAsset;
        }

        private static T LoadAssetFromBundle<T>(AssetBundle bundle,string bundleName,string assetName) where T : UnityEngine.Object
        {
            T loadedAsset = bundle.LoadAsset<T>(assetName);
            string key = GenerateAssetKey(bundleName, assetName);
            _LOADED_ASSETS.Add(key, loadedAsset);
            return loadedAsset;
        }

        private static async Task<AssetBundle> LoadBundle<T>(string bundleName) where T : UnityEngine.Object
        {
            AssetBundle bundle = await new BundleResourceRequest().LoadAssetBundle(bundleName);
            _LOADED_BUNDLES.Add(bundleName, bundle);
            return bundle;
        }

        public static async Task UnloadBundle(string bundleName)
        {
            if (!IsBundleLoaded(bundleName))
                return;

            AssetBundle bundle = _LOADED_BUNDLES[bundleName];
            RemoveAllAssetReferencesOfBundle(bundleName,bundle);
            _LOADED_BUNDLES.Remove(bundleName);
            AsyncOperation unloadOperation = bundle.UnloadAsync(true);

            while (unloadOperation.isDone)
                await Task.Yield();
        }

        private static bool IsBundleLoaded(string bundleName)
        {
            return _LOADED_BUNDLES.ContainsKey(bundleName);
        }

        private static bool IsAssetLoaded(string assetName)
        {
            return _LOADED_ASSETS.ContainsKey(assetName);
        }

        private static void RemoveAllAssetReferencesOfBundle(string bundleName,AssetBundle bundle)
        {
            string[] assetNames = bundle.GetAllAssetNames();
            for (int i = 0; i < assetNames.Length; i++)
            {
                string key = bundleName + assetNames[i];
                if(!_LOADED_ASSETS.ContainsKey(key))
                    _LOADED_ASSETS.Remove(key);
            }
        }

        private static string GenerateAssetKey(string bundleName, string assetName)
        {
            return bundleName + assetName;
        }

    }

}