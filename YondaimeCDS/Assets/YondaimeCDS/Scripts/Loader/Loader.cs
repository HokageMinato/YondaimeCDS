
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
                string key = bundleName + assetName;
                return (T)_LOADED_ASSETS[key];
            }
            
            AssetBundle bundle = await new BundleResourceRequest().LoadAssetBundle(bundleName);
            _LOADED_BUNDLES.Add(bundleName,bundle);

            T loadedAsset = bundle.LoadAsset<T>(assetName);
            _LOADED_ASSETS.Add(assetName,loadedAsset);

            return loadedAsset;
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

    }

}