using UnityEngine;

namespace YondaimeCDS {

    public static class ManifestTracker
    {
	    private static SerializedAssetManifest _LOCAL_ASSET_MANIFEST;
	    private static ScriptManifest _LOCAL_SCRIPT_MANIFEST;
	    private static HashManifest _LOCAL_HASH_MANIFEST;

	    public static SerializedAssetManifest LocalAssetManifest { get { return _LOCAL_ASSET_MANIFEST; } }
	    public static ScriptManifest LocalScriptManifest { get { return _LOCAL_SCRIPT_MANIFEST; } }
	    public static HashManifest LocalHashManifest { get { return _LOCAL_HASH_MANIFEST; } }

	    
	    public static void Initialize(string localSerializedScriptManifest)
	    {
		    _LOCAL_SCRIPT_MANIFEST = IOUtils.Deserialize<ScriptManifest>(localSerializedScriptManifest);
		    _LOCAL_ASSET_MANIFEST = IOUtils.LoadFromLocalDisk<SerializedAssetManifest>(Constants.ASSET_MANIFEST);
		    _LOCAL_HASH_MANIFEST = IOUtils.LoadFromLocalDisk<HashManifest>(Constants.MANIFEST_HASH);
	    }
	    
	    public static void UpdateHashManifestDiskContents(HashManifest serverHashManifest)
	    {
		    _LOCAL_HASH_MANIFEST = serverHashManifest;
		    IOUtils.SaveObjectToLocalDisk(LocalHashManifest,Constants.MANIFEST_HASH);
	    }
	    
	    public static void UpdateAssetManifestDiskContents()
	    {
		    IOUtils.SaveObjectToLocalDisk(LocalAssetManifest,Constants.ASSET_MANIFEST);
	    }
	    
	    public static void CreateLocalManifestFrom(SerializedAssetManifest serverManifest)
	    {
		    _LOCAL_ASSET_MANIFEST = serverManifest;
		    UpdateAssetManifestDiskContents();
	    }

	    
    }

}