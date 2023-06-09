using System;
using System.Collections.Generic;
using UnityEngine;


namespace YondaimeCDS {

    public class SerializedAssetManifest : ISerializationCallbackReceiver
    {

        [SerializeField]
        private List<string> m_Keys;

        [SerializeField]
        private List<BundleDetails> m_Values;

        
        internal IReadOnlyList<string> BundleNames { get { return m_Keys; } }



        private Dictionary<string, BundleDetails> m_Details;


        internal void FilterAlreadyUpdatedBundles(SerializedAssetManifest serverManifest, ref List<string> scriptFilteredBundleList)
        {
            for (int i = 0; i < scriptFilteredBundleList.Count;)
            {
                string bundleName = scriptFilteredBundleList[i];

                if (!m_Details.ContainsKey(bundleName)) 
                {
                    i++;
                    continue;
                }

                Hash128 serverHashValue = serverManifest.m_Details[bundleName].Hash;
                Hash128 localHashValue = m_Details[bundleName].Hash;

                if (serverHashValue.Equals(localHashValue))
                {
                    scriptFilteredBundleList.RemoveAt(i);
                    continue;
                }

                i++;
            }

        }

        internal void UpdateManifestData(SerializedAssetManifest serverManifest, ref List<string> scriptFilteredBundleList)
        {
            
            Dictionary<string, BundleDetails> updates = serverManifest.m_Details;

            for (int i = 0; i < scriptFilteredBundleList.Count; i++)
            {
                if(m_Details.ContainsKey(scriptFilteredBundleList[i]))  
                    m_Details[scriptFilteredBundleList[i]] = updates[scriptFilteredBundleList[i]];
                else
                    m_Details.Add(scriptFilteredBundleList[i], updates[scriptFilteredBundleList[i]]);

                BundleSystem.Log($"updated details for {scriptFilteredBundleList[i]}");
            }

        }

        internal string[] GetBundleDependencies(AssetHandle assetHandle) 
        {
            string bundleName = assetHandle.BundleName;
            if (m_Details.ContainsKey(bundleName)) 
            {
                return m_Details[bundleName].Dependencies;
            }

            return default;
        }

       
        public void OnAfterDeserialize()
        {
            m_Details = new Dictionary<string, BundleDetails>();
            int count = Math.Min(m_Keys.Count, m_Values.Count);

            for (int i = 0; i != count; i++)
            {
                m_Details.Add(m_Keys[i], m_Values[i]);
            }
        }

        public void OnBeforeSerialize()
        {
            m_Keys = new List<string>();
            m_Values = new List<BundleDetails>();
            foreach (KeyValuePair<string, BundleDetails> detail in m_Details)
            {
                m_Keys.Add(detail.Key);
                m_Values.Add(detail.Value);
            }
        }


        internal double GetBundleSize(string bundleName) 
        {
            return m_Details[bundleName].BundleSize;
        }

       

        #if UNITY_EDITOR
        public List<string> Keys { get { return m_Keys; } }

        public void SetBundleSizeOf(string bundleName,double bundleSize)
        {
            BundleDetails details = m_Details[bundleName];
            details.BundleSize = bundleSize;
            m_Details[bundleName] = details;
        }

        //public void SetLocalBundleList(List<string> bundles) 
        //{ 
        //    localBundles = bundles;
        //}
        #endif

    }

}