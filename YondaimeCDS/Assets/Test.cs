using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OdinSerializer;
using System.Runtime.Serialization;

public class Test : ScriptableObject //,ISerializationCallbackReceiver
{
	

 //   void ISerializationCallbackReceiver.OnAfterDeserialize()
	//{
	//	context = new SerializationContext();
	//	StreamingContext sc = new StreamingContext(StreamingContextStates.CrossProcess, myObject);

	//	// Make Odin deserialize the serializationData field's contents into this instance.
	//	UnitySerializationUtility.DeserializeUnityObject(this, ref this.serializationData, cachedContext.Value);
	//}

	//void ISerializationCallbackReceiver.OnBeforeSerialize()
	//{
	//	// Whether to always serialize fields that Unity will also serialize. By default, this parameter is false, and OdinSerializer will only serialize fields that it thinks Unity will not handle.
	//	bool serializeUnityFields = false;
		
	//	// Make Odin serialize data from this instance into the serializationData field.
	//	UnitySerializationUtility.SerializeUnityObject(this, ref this.serializationData, serializeUnityFields, cachedContext.Value);
	//}
}
