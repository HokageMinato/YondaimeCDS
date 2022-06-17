using System.Collections.Generic;
using UnityEngine;

namespace YondaimeCDS {

    internal static class ManifestTracker
    {
	    private static SerializedAssetManifest _LOCAL_ASSET_MANIFEST;
	    private static ScriptManifest _LOCAL_SCRIPT_MANIFEST;
	    private static HashManifest _LOCAL_HASH_MANIFEST;

	    internal static SerializedAssetManifest LocalAssetManifest { get { if (_LOCAL_ASSET_MANIFEST == null) 
					{
					Debug.LogError("EMPTY MANIFEST ACCESS");
					} return	_LOCAL_ASSET_MANIFEST; } }
	    internal static ScriptManifest LocalScriptManifest { get { return _LOCAL_SCRIPT_MANIFEST; } }
	    internal static HashManifest LocalHashManifest { get { return _LOCAL_HASH_MANIFEST; } }

		internal static bool IsDefaultManifestLoaded = false;
		
	    
	    internal static void Initialize(BundleSystemConfig systemConfig)
	    {
			_LOCAL_SCRIPT_MANIFEST = Utils.Deserialize<ScriptManifest>(systemConfig.serializedScriptManifest);
			_LOCAL_HASH_MANIFEST = Utils.LoadFromLocalDisk<HashManifest>(Constants.MANIFEST_HASH);
		    _LOCAL_ASSET_MANIFEST = Utils.LoadFromLocalDisk<SerializedAssetManifest>(Constants.ASSET_MANIFEST);
		}
	    
	    internal static void CreateOrReplaceLocalHash(HashManifest serverHashManifest)
	    {
		    _LOCAL_HASH_MANIFEST = serverHashManifest;
		    Utils.SaveObjectToLocalDisk(LocalHashManifest,Constants.MANIFEST_HASH);
	    }
	    
	    internal static void UpdateAssetManifestDiskContents()
	    {
		    Utils.SaveObjectToLocalDisk(LocalAssetManifest,Constants.ASSET_MANIFEST);
	    }

        internal static void CreateLocalManifestFrom(SerializedAssetManifest defaultManifest)
        {
            _LOCAL_ASSET_MANIFEST = defaultManifest;
            UpdateAssetManifestDiskContents();
        }
	    
    }

}