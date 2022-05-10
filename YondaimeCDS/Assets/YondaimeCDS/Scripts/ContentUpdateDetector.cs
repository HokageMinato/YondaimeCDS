using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace YondaimeCDS
{
    public class ContentUpdateDetector
    {
        private static string MANIFEST_HASH
        {
            get { return Config.MANIFEST_HASH; }
        }

        private static string ASSET_MANIFEST
        {
            get { return Config.ASSET_MANIFEST; }
        }

        private static string SCRIPT_MANIFEST
        {
            get { return Config.SCRIPT_MANIFEST; }
        }


        private static ScriptManifest LocalScriptManifest
        {
            get { return Downloader.LocalScriptManifest; }
        }

        private static HashManifest LocalHashManifest
        {
            get { return Downloader.LocalHashManifest; }
        }

        private static SerializedAssetManifest LocalAssetManifest
        {
            get { return Downloader.LocalAssetManifest; }
            set => Downloader.LocalAssetManifest = value;
        }

        private ScriptManifest _serverScriptManifest;
        private HashManifest _serverHashManifest;
        private SerializedAssetManifest _serverAssetManifest;

        private List<string> _compatibleBundles = new List<string>();


        public async Task<List<string>> GetUpdates()
        {
            if (IsUnitialized())
                return null;
            
            await DownloadManifestHash();
            
            if (ScriptUpdateDetected())
            {
                await DownloadScriptManifest();
                FilterScriptIncompitableBundles();
            }

            if (LocalAssetManifest == null || AssetManifestUpdateDetected())
            {
                await DownloadAssetManifest();
                FilterNonUpdatedBundles();

                if (BundleUpdatesPresent())
                    UpdateLocalAssetManifest();
            }
             
            //update hash.. asset value only..
            return LocalAssetManifest.PendingUpdates;
        }


        private async Task DownloadManifestHash()
        {
            byte[] manifestHashBuffer = await DownloadFromServer(MANIFEST_HASH);
            if (manifestHashBuffer == null)
            {
                Debug.Log("Empty manifestHash recieved");
            }
            
            _serverHashManifest = IOUtils.Deserialize<HashManifest>(IOUtils.BytesToString(manifestHashBuffer));
        }

        
        #region UPDATE_CHECK

        private bool ScriptUpdateDetected()
        {
            return _serverHashManifest.ScriptHash != LocalHashManifest.ScriptHash;
        }

        private bool IsUnitialized()
        {
            return LocalHashManifest == null;
        }

        private bool AssetManifestUpdateDetected()
        {
            return _serverHashManifest.AssetHash != LocalHashManifest.AssetHash;
        }

        private bool BundleUpdatesPresent()
        {
            return _compatibleBundles.Count > 0;
        }

        #endregion

        
        #region HASH_MANIFEST_MANAGEMENT

        private void CreateLocalHashManifest()
        {
            Downloader.CreateHashManifestDiskContents(_serverHashManifest);
        }

        #endregion

        
        
        #region ASSET_MANIFEST_MANAGEMENT


        private async Task DownloadAssetManifest()
        {
            byte[] serverManifestBuffer = await DownloadFromServer(ASSET_MANIFEST);
            if (serverManifestBuffer == null)
            {
                Debug.Log("Empty bytes recieved at hashManifest, check source");
                return;
            }

            _serverAssetManifest = IOUtils.Deserialize<SerializedAssetManifest>(IOUtils.BytesToString(serverManifestBuffer));
        }
        
        private void FilterNonUpdatedBundles()
        {
            bool scriptUpdatesPresent = ScriptUpdateDetected();
            LocalAssetManifest.GenerateUpdateList(_serverAssetManifest, scriptUpdatesPresent, ref _compatibleBundles);
        }

        private void UpdateLocalAssetManifest()
        {
            if (LocalAssetManifest == null)
                LocalAssetManifest = _serverAssetManifest;
            
            LocalAssetManifest.UpdateManifestData(_serverAssetManifest, ref _compatibleBundles);
            Downloader.WriteAssetManifestToDisk();
        }

        #endregion


        #region SCRIPT_MANIFEST_MANAGEMENT

        private void FilterScriptIncompitableBundles()
        {
            _compatibleBundles = LocalScriptManifest.GetCompatibleBundleList(_serverScriptManifest);
        }


        private async Task DownloadScriptManifest()
        {
            byte[] scriptManifestBuffer = await DownloadFromServer(SCRIPT_MANIFEST);
            if (scriptManifestBuffer == null)
            {
                Debug.Log("Empty script manifest received from server");
                return;
            }

            _serverScriptManifest = IOUtils.Deserialize<ScriptManifest>(IOUtils.BytesToString(scriptManifestBuffer));
        }

        #endregion

        
        
        #region IO_OPERATION

        private async Task<byte[]> DownloadFromServer(string manifestName)
        {
            return await new DownloadHandler().DownloadContent(manifestName);
            //Dont save, every first change detection will replace old with new and next run will mark incompitables compitable.
            //Write only if compatibility check passes.
        }

       

        #endregion
    }
}