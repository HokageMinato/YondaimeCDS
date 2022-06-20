using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace YondaimeCDS {

	internal class BundleResourceRequest
	{
		private AssetHandle _loadHandle;


		internal AssetBundle LoadAssetBundle(AssetHandle loadHandle) 
		{
			_loadHandle = loadHandle;
			AssetBundle bundle;

			if (ContentTracker.IsBundleAvailableInBuild(_loadHandle))
			{
				bundle = TryLoadFromStreamedStorage();
				return bundle; 
			}

			bundle = TryLoadFromPersistantStorage();
			return bundle;
		}

		private AssetBundle TryLoadFromPersistantStorage() 
		{
			Debug.Log($"Loading {_loadHandle.BundleName} from Persistant Storage");
			return LoadBundleFromPath(Path.Combine(BundleSystemConfig.STORAGE_PATH, _loadHandle.BundleName));
		}
		
		private  AssetBundle TryLoadFromStreamedStorage() 
		{
			Debug.Log($"Loading {_loadHandle.BundleName} from Local Storage");
			return LoadBundleFromPath(Path.Combine(BundleSystemConfig.STREAM_PATH, _loadHandle.BundleName));
		}

		private async Task<AssetBundle> LoadBundleFromPathAsync(string path) 
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
		
		private AssetBundle LoadBundleFromPath(string path) 
		{
			
			AssetBundle bundle= AssetBundle.LoadFromFile(path);
			return bundle;
		}




	}

}