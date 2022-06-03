using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace YondaimeCDS {

	internal class BundleResourceRequest
	{
		private AssetHandle _loadHandle;

		internal async Task<AssetBundle> LoadAssetBundle(AssetHandle loadHandle) 
		{
			_loadHandle = loadHandle;

			AssetBundle bundle = await TryLoadFromPersistantStorage();
			if (bundle == null)
				bundle = await TryLoadFromStreamedStorage();

			return bundle;
		}

		private async Task<AssetBundle> TryLoadFromPersistantStorage() 
		{
			return await LoadBundleFromPath(Path.Combine(BundleSystemConfig.STORAGE_PATH, _loadHandle.BundleName));
		}
		
		private async Task<AssetBundle> TryLoadFromStreamedStorage() 
		{
			return await LoadBundleFromPath(Path.Combine(BundleSystemConfig.STREAM_PATH, _loadHandle.BundleName));
		}

		private async Task<AssetBundle> LoadBundleFromPath(string path) 
		{
			AssetBundleCreateRequest bundleCreationRequest = AssetBundle.LoadFromFileAsync(path);
			
			if(bundleCreationRequest == null)
				return null;


			while (!bundleCreationRequest.isDone)
			{
				_loadHandle.OnOperationProgressChanged(bundleCreationRequest.progress);
				await Task.Yield();
			}

			return bundleCreationRequest.assetBundle;
		}




	}

}