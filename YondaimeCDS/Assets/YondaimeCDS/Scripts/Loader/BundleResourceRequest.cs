using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace YondaimeCDS {

	public class BundleResourceRequest
	{
		
		public async Task<AssetBundle> LoadAssetBundle(string assetName) 
		{
			string fileName = Path.Combine(Config.STORAGE_PATH, assetName);
			AssetBundleCreateRequest bundleCreationRequest = AssetBundle.LoadFromFileAsync(fileName);

			while (!bundleCreationRequest.isDone)
				await Task.Yield();

			return bundleCreationRequest.assetBundle;			
			
		}
		
	}

}