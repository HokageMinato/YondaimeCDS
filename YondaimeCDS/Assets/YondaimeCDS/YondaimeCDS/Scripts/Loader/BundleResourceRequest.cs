using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace YondaimeCDS {

	public class BundleResourceRequest
	{
		
		public async Task<AssetBundle> LoadAssetBundle(string assetName) 
		{
			AssetBundle bundle = await TryLoadFromPersistantStorage(assetName);
			if (bundle == null)
				bundle = await TryLoadFromStreamedStorage(assetName);

			return bundle;
		}

		private async Task<AssetBundle> TryLoadFromPersistantStorage(string bundleName) 
		{
			string persistantStoragePath = Path.Combine(BundleSystemConfig.STORAGE_PATH, bundleName);
			return await LoadBundleFromPath(persistantStoragePath);
		}
		
		private async Task<AssetBundle> TryLoadFromStreamedStorage(string bundleName) 
		{
			string streamedStoragePath = Path.Combine(BundleSystemConfig.STREAM_PATH, bundleName);
			return await LoadBundleFromPath(streamedStoragePath);
		}

		private async Task<AssetBundle> LoadBundleFromPath(string path) 
		{
			AssetBundleCreateRequest bundleCreationRequest = AssetBundle.LoadFromFileAsync(path);
			
			if(bundleCreationRequest == null)
				return null;

			while (!bundleCreationRequest.isDone)
				await Task.Yield();

			return bundleCreationRequest.assetBundle;
		}




	}

}