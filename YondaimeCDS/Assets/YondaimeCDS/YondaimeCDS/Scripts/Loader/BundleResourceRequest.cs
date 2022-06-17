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
			AssetBundle bundle;

			if (ContentTracker.IsBundleAvailableInBuild(_loadHandle))
			{
				bundle = await TryLoadFromStreamedStorage();
				return bundle; 
			}

			bundle = await TryLoadFromPersistantStorage();
			return bundle;
		}

		private async Task<AssetBundle> TryLoadFromPersistantStorage() 
		{
			Debug.Log($"Loading {_loadHandle.BundleName} from Persistant Storage");
			return await LoadBundleFromPath(Path.Combine(BundleSystemConfig.STORAGE_PATH, _loadHandle.BundleName));
		}
		
		private async Task<AssetBundle> TryLoadFromStreamedStorage() 
		{
			Debug.Log($"Loading {_loadHandle.BundleName} from Local Storage");
			return await LoadBundleFromPath(Path.Combine(BundleSystemConfig.STREAM_PATH, _loadHandle.BundleName));
		}

		private async Task<AssetBundle> LoadBundleFromPath(string path) 
		{
			
			AssetBundleCreateRequest bundleCreationRequest= AssetBundle.LoadFromFileAsync(path);
		
			if (bundleCreationRequest == null)
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