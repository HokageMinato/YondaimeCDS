using System.Collections.Generic;
using UnityEngine;

namespace YondaimeCDS {

    internal static class ManifestTracker
    {
	    private static SerializedAssetManifest _LOCAL_ASSET_MANIFEST;
	    private static ScriptManifest _LOCAL_SCRIPT_MANIFEST;
	    private static HashManifest _LOCAL_HASH_MANIFEST;

	    internal static SerializedAssetManifest LocalAssetManifest { get { return _LOCAL_ASSET_MANIFEST; } }
	    internal static ScriptManifest LocalScriptManifest { get { return _LOCAL_SCRIPT_MANIFEST; } }
	    internal static HashManifest LocalHashManifest { get { return _LOCAL_HASH_MANIFEST; } }

	    
	    internal static void Initialize(string localSerializedScriptManifest)
	    {
		    _LOCAL_SCRIPT_MANIFEST = IOUtils.Deserialize<ScriptManifest>(localSerializedScriptManifest);
		    _LOCAL_ASSET_MANIFEST = IOUtils.LoadFromLocalDisk<SerializedAssetManifest>(Constants.ASSET_MANIFEST);
		    _LOCAL_HASH_MANIFEST = IOUtils.LoadFromLocalDisk<HashManifest>(Constants.MANIFEST_HASH);
	    }
	    
	    internal static void UpdateHashManifestDiskContents(HashManifest serverHashManifest)
	    {
		    _LOCAL_HASH_MANIFEST = serverHashManifest;
		    IOUtils.SaveObjectToLocalDisk(LocalHashManifest,Constants.MANIFEST_HASH);
	    }
	    
	    internal static void UpdateAssetManifestDiskContents()
	    {
		    IOUtils.SaveObjectToLocalDisk(LocalAssetManifest,Constants.ASSET_MANIFEST);
	    }
	    
	    internal static void CreateLocalManifestFrom(SerializedAssetManifest serverManifest)
	    {
		    _LOCAL_ASSET_MANIFEST = serverManifest;
		    UpdateAssetManifestDiskContents();
	    }

		
	    
    }

}