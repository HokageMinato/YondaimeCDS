using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Build.Pipeline;


namespace YondaimeCDS {

    public class SerializedAssetManifest : ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<string> m_Keys;

        [SerializeField]
        private List<BundleDetails> m_Values;

        [SerializeField]
        private List<string> pendingUpdates = new List<string>();

        //public string bit;

        public List<string> PendingUpdates { get { return pendingUpdates; } }
        public List<string> Keys { get { return m_Keys; } }

        private Dictionary<string, BundleDetails> m_Details;

        public void GenerateUpdateList(SerializedAssetManifest serverManifest,ref List<string> scriptFilteredBundleList)
        {
            for (int i = 0; i < scriptFilteredBundleList.Count;)
            {
                string bundleName = scriptFilteredBundleList[i];
                Hash128 serverHashValue = serverManifest.m_Details[bundleName].Hash;
                Hash128 localHashValue = m_Details[bundleName].Hash;

                if (serverHashValue.Equals(localHashValue)) 
                {
                    scriptFilteredBundleList.RemoveAt(i);
                    continue;
                }

                i++;
            }

            Debug.Log("No bundle updates");
        }

        public void UpdateManifestData(SerializedAssetManifest serverManifest, ref List<string> scriptFilteredBundleList)
        {
            PendingUpdates.AddRange(scriptFilteredBundleList);
            Dictionary<string, BundleDetails> updates = serverManifest.m_Details;

            for (int i = 0; i < scriptFilteredBundleList.Count; i++)
            {
                m_Details[scriptFilteredBundleList[i]] = updates[scriptFilteredBundleList[i]];
                Debug.Log($"updated details for {scriptFilteredBundleList[i]}");
            }
            
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

        
        

    }

}