using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OdinSerializer;
using Object = UnityEngine.Object;
using System.IO;
using System.Text;
using System.Runtime.Serialization;



public class SerializeTest : MonoBehaviour 
{

    public GameObject objectToSerialize;

    public List<Object> ob= new List<Object>();
    public byte[] data;
    DataFormat format = DataFormat.Binary;
    public string json;


    

    [ContextMenu("SerializeItem")]
    public void TestSerialize()
    {
            data = SerializationUtility.SerializeValue(objectToSerialize, format, out ob);
            json = System.Text.Encoding.UTF8.GetString(data);
    }

    [ContextMenu("DeserializeItem")]
    public void DeserizTest() 
    {
        byte[] bt = data;
        // If you have a string to deserialize, get the bytes using UTF8 encoding
        // var bytes = System.Text.Encoding.UTF8.GetBytes(jsonString);
        Instantiate(SerializationUtility.DeserializeValue<GameObject>(data, format, ob));
    }


    private List<Object> GetObjectListOf(GameObject ob) 
    {
        Component[] comp = objectToSerialize.GetComponents<Component>();
        List<Object> list = new List<Object>();

        foreach (var item in comp)
        {
            list.Add(item);
           // Debug.Log($"Added {item.GetType()}");
        }

        return list;
    }

    


}
