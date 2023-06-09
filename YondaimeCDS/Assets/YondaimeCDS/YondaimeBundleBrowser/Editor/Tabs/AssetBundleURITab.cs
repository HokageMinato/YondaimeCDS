using UnityEditor;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using YondaimeCDS;

namespace AssetBundleBrowser
{

    internal class AssetBundleURITab
    {

        [SerializeField]
        private Vector2 m_ScrollPosition;

        internal AssetBundleURITabData m_UserData;

        internal AssetBundleURITab()
        {
            m_UserData = new AssetBundleURITabData();
        }


        internal void OnEnable()
        {

            //LoadData...
            var dataPath = System.IO.Path.GetFullPath(".");
            dataPath = dataPath.Replace("\\", "/");
            dataPath += "/Library/AssetBundleBrowserURI.dat";

            if (File.Exists(dataPath))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(dataPath, FileMode.Open);
                var data = bf.Deserialize(file) as AssetBundleURITabData;
                if (data != null)
                    m_UserData = data;
                file.Close();
            }
        }

        internal void OnDisable()
        {
            var dataPath = Path.GetFullPath(".");
            dataPath = dataPath.Replace("\\", "/");
            string localSettingSave =  dataPath + "/Library/AssetBundleBrowserURI.dat";

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(localSettingSave);

            bf.Serialize(file, m_UserData);
            file.Close();


            BundleSystemConfig config = Utils.LoadFromResourcesTextAsset<BundleSystemConfig>(Constants.SYSTEM_SETTINGS);
            config.remoteURL = m_UserData.remoteURL;
            config.autoUpdateCatelog = m_UserData.autoUpdateCatelog;
            ManifestGenerator.WriteConfigToResources(config);

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

        }



        internal void OnGUI()
        {
            m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
            var centeredStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
            centeredStyle.alignment = TextAnchor.UpperCenter;
            
            GUILayout.Label(new GUIContent("Configuration"), centeredStyle);
            EditorGUILayout.Space();
            GUILayout.BeginVertical();

            //URL Settings
            EditorGUILayout.LabelField(new GUIContent("URL Settings"));
            EditorGUILayout.Space();
            var newPath = EditorGUILayout.TextField("Remote URL", m_UserData.remoteURL);
            if (!string.IsNullOrEmpty(newPath) && newPath != m_UserData.remoteURL)
                m_UserData.remoteURL = newPath;
            
            //AutoUpdate Toggle
            EditorGUILayout.Space();
            m_UserData.autoUpdateCatelog = GUILayout.Toggle(m_UserData.autoUpdateCatelog,new GUIContent("Auto Update Manifests"));

            

            GUILayout.EndVertical();
            EditorGUILayout.EndScrollView();


        }

    }


    [System.Serializable]
    internal class AssetBundleURITabData
    {
        internal string remoteURL;
        internal bool autoUpdateCatelog;
    }
}