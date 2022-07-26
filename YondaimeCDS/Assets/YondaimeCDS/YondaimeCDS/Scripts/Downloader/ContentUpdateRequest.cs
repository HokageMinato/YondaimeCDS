using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace YondaimeCDS
{
    internal class ContentUpdateRequest
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


        internal async Task<IReadOnlyList<string>> GetServerAssetUpdatesList()
        {
            await DownloadManifestHash();
            if (!ServerManifestHashPresent())
                return _compatibleBundles;

            if (!AssetManifestHashUpdateDetected())
                return _compatibleBundles;

            await DownloadScriptManifest();
            FilterScriptIncompitableBundles();

            await DownloadAssetManifest();
            if (LocalAssetManifest != null)
                FilterAlreadyUpdatedBundles();

            if (BundleUpdatesPresent())
            {
                CreateOrUpdateLocalAssetManifest();
                CreateOrUpdateLocalHashManifest();
            }
            
            return _compatibleBundles;
        }


        private async Task DownloadManifestHash()
        {
            byte[] manifestHashBuffer = await DownloadFromServer(MANIFEST_HASH);
            if (manifestHashBuffer == null)
            {
                BundleSystem.Log("Empty manifestHash recieved");
                return;
            }
            
            _serverHashManifest = Utils.Deserialize<HashManifest>(Utils.BytesToString(manifestHashBuffer));
        }

        
        #region UPDATE_CHECK

        private bool LocalManifestHashPresent() 
        {
            return LocalHashManifest != null;
        }
        
        private bool ServerManifestHashPresent()
        {
            return _serverHashManifest != null;
        }

        private bool AssetManifestHashUpdateDetected()
        {
            if (!LocalManifestHashPresent())
                return true;

            if (!ServerManifestHashPresent())
                return false;


            return _serverHashManifest._assetHash != LocalHashManifest._assetHash;
        }

        private bool BundleUpdatesPresent()
        {
            BundleSystem.Log($"Updates present {_compatibleBundles.Count > 0}");
            return _compatibleBundles.Count > 0;
        }

        #endregion

        
        #region HASH_MANIFEST_MANAGEMENT

        private void CreateOrUpdateLocalHashManifest()
        {
            ManifestTracker.CreateOrReplaceLocalHash(_serverHashManifest);
        }

        #endregion

        
        
        #region ASSET_MANIFEST_MANAGEMENT


        private async Task DownloadAssetManifest()
        {
            byte[] serverManifestBuffer = await DownloadFromServer(ASSET_MANIFEST);
            if (serverManifestBuffer == null)
            {
                BundleSystem.Log("Empty bytes recieved for AssetManifest, check source");
                return;
            }

            _serverAssetManifest = Utils.Deserialize<SerializedAssetManifest>(Utils.BytesToString(serverManifestBuffer));
        }
        
        private void FilterAlreadyUpdatedBundles()
        {
           LocalAssetManifest.FilterAlreadyUpdatedBundles(_serverAssetManifest, ref _compatibleBundles);
        }

        private void CreateOrUpdateLocalAssetManifest()
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
                BundleSystem.Log("Empty script manifest received from server");
                return;
            }

            _serverScriptManifest = Utils.Deserialize<ScriptManifest>(Utils.BytesToString(scriptManifestBuffer));
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