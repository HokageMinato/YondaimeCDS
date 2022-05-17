using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace YondaimeCDS
{
    public static class Loader
    {
        private static Dictionary<string, AssetBundle> _LOADED_BUNDLES = new Dictionary<string, AssetBundle>();
        private static Dictionary<string, UnityEngine.Object> _LOADED_ASSETS = new Dictionary<string, UnityEngine.Object>();


        #region PRIVATE_PROPERTIES

        private static SerializedAssetManifest LocalAssetManifest
        {
            get { return ManifestTracker.LocalAssetManifest; }
        }

        #endregion


        public static async Task<T> LoadAsset<T>(string bundleName, string assetName) where T : UnityEngine.Object
        {

            if (IsBundleLoaded(bundleName))
            {
                string key = GenerateAssetKey(bundleName, assetName);
                if (IsAssetLoaded(key))
                {
                    Debug.Log("Returning cached asset");
                    return (T)_LOADED_ASSETS[key];
                }

                Debug.Log("loading asset from cached bundle");
                AssetBundle loadedBundle = _LOADED_BUNDLES[bundleName];
                return LoadAssetFromBundle<T>(loadedBundle, bundleName, assetName);
            }


            AssetBundle bundle = await LoadBundle<T>(bundleName);
            if(bundle == null)
                return null;

            T loadedAsset = LoadAssetFromBundle<T>(bundle, bundleName, assetName);
            return loadedAsset;
        }

        private static T LoadAssetFromBundle<T>(AssetBundle bundle, string bundleName, string assetName)
            where T : UnityEngine.Object
        {
            T loadedAsset = bundle.LoadAsset<T>(assetName);
            string key = GenerateAssetKey(bundleName, assetName);
            _LOADED_ASSETS.Add(key, loadedAsset);
            return loadedAsset;
        }

        private static async Task<AssetBundle> LoadBundle<T>(string bundleName) where T : UnityEngine.Object
        {
            AssetBundle bundle = await new BundleResourceRequest().LoadAssetBundle(bundleName);
            if (bundle == null)
            {
                Debug.LogError($"Invalid bundle load request {bundle}");
                return null;
            }

            _LOADED_BUNDLES.Add(bundleName, bundle);
            return bundle;
        }

        public static void UnloadBundle(string bundleName)
        {
            if (!IsBundleLoaded(bundleName))
                return;

            AssetBundle bundle = _LOADED_BUNDLES[bundleName];
            RemoveAllAssetReferencesOfBundle(bundleName, bundle);
            _LOADED_BUNDLES.Remove(bundleName);
            bundle.Unload(true);
        }

        private static bool IsBundleLoaded(string bundleName)
        {
            return _LOADED_BUNDLES.ContainsKey(bundleName);
        }

        private static bool IsAssetLoaded(string assetName)
        {
            return _LOADED_ASSETS.ContainsKey(assetName);
        }

        private static void RemoveAllAssetReferencesOfBundle(string bundleName, AssetBundle bundle)
        {
            string[] assetNames = bundle.GetAllAssetNames();
            for (int i = 0; i < assetNames.Length; i++)
            {
                string key = bundleName + assetNames[i];
                if (_LOADED_ASSETS.ContainsKey(key))
                    _LOADED_ASSETS.Remove(key);
            }
        }

        private static string GenerateAssetKey(string bundleName, string assetName)
        {
            return bundleName + assetName;
        }

      

    }
}