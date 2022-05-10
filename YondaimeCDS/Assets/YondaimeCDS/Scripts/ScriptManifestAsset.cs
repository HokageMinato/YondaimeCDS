using UnityEngine;

namespace YondaimeCDS {

	[CreateAssetMenu(fileName = "ScriptManifestAsset", menuName = "YondaimeCDS/ScriptManifest")]
    public class ScriptManifestAsset : ScriptableObject
    {
	    [SerializeField] string serializedScriptManifest;
	    private ScriptManifest _scriptManifest;

	    public ScriptManifest GetManifest()
	    {
		    if (_scriptManifest == null)
			    _scriptManifest = IOUtils.Deserialize<ScriptManifest>(serializedScriptManifest);

		    return _scriptManifest;
	    }
	    
    }

}