using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace YondaimeCDS
{
    public class ContentUpdateDetector
    {
        private static string MANIFEST_HASH
        {
            get { return Constants.MANIFEST_HASH; }
        }

        private static string ASSET_MANIFEST
        {
            get { return Constants.ASSET_MANIFEST; }
        }

        private static string SCRIPT_MANIFEST
        {
            get { return Constants.SCRIPT_MANIFEST; }
        }


        private static ScriptManifest LocalScriptManifest
        {
            get { return ManifestTracker.LocalScriptManifest; }
        }

        private static HashManifest LocalHashManifest
        {
            get { return ManifestTracker.LocalHashManifest; }
        }

        private static SerializedAssetManifest LocalAssetManifest
        {
            get { return ManifestTracker.LocalAssetManifest; }
        }

        private ScriptManifest _serverScriptManifest;
        private HashManifest _serverHashManifest;
        private SerializedAssetManifest _serverAssetManifest;

        private List<string> _compatibleBundles = new List<string>();


        public async Task<IReadOnlyList<string>> GetUpdates()
        {
            await DownloadManifestHash();

            if (!ServerManifestHashPresent() && !LocalAssetManifestPresent())
                return null;

            if (!AssetManifestHashUpdateDetected())
                return LocalAssetManifest.PendingUpdates;


            await DownloadScriptManifest();
            FilterScriptIncompitableBundles();

            await DownloadAssetManifest();
            CheckForBundleUpdates();

            if (BundleUpdatesPresent())
            {
                UpdateLocalAssetManifest();
                UpdateLocalHashManifest();
            }
            
            return LocalAssetManifest.PendingUpdates;
        }


        private async Task DownloadManifestHash()
        {
            byte[] manifestHashBuffer = await DownloadFromServer(MANIFEST_HASH);
            if (manifestHashBuffer == null)
            {
                Debug.Log("Empty manifestHash recieved");
                return;
            }
            
            _serverHashManifest = IOUtils.Deserialize<HashManifest>(IOUtils.BytesToString(manifestHashBuffer));
        }

        
        #region UPDATE_CHECK

        private bool LocalAssetManifestPresent()
        {
            return LocalAssetManifest != null;
        }
        
        private bool ServerManifestHashPresent()
        {
            return _serverHashManifest != null;
        }

        private bool AssetManifestHashUpdateDetected()
        {
            if (!LocalAssetManifestPresent())
                return true;

            if(!ServerManifestHashPresent())
                return false;
            
            return _serverHashManifest.AssetHash != LocalHashManifest.AssetHash;
        }

        private bool BundleUpdatesPresent()
        {
            return _compatibleBundles.Count > 0;
        }

        

        #endregion

        
        #region HASH_MANIFEST_MANAGEMENT

        private void UpdateLocalHashManifest()
        {
            ManifestTracker.UpdateHashManifestDiskContents(_serverHashManifest);
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
        
        private void CheckForBundleUpdates()
        {
            if(LocalAssetManifestPresent())
                LocalAssetManifest.GenerateUpdateList(_serverAssetManifest, ref _compatibleBundles);
        }

        private void UpdateLocalAssetManifest()
        {
            if(LocalAssetManifest == null)
                ManifestTracker.CreateLocalManifestFrom(_serverAssetManifest);    
            
            LocalAssetManifest.UpdateManifestData(_serverAssetManifest, ref _compatibleBundles);
            ManifestTracker.UpdateAssetManifestDiskContents();
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
            return await new BundleDownloadHandle().DownloadContent(manifestName);
            //Dont save, every first change detection will replace old with new and next run will mark incompitables compitable.
            //Write only if compatibility check passes.
        }

       

        #endregion
    }
}