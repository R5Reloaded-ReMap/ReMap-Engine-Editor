using UnityEngine;
using System.Collections.Generic;

[ System.Serializable ]
public struct SerializableKeyValue
{
    public string Key;
    public string Value;
}

public class EntitiesKeyValues : MonoBehaviour
{
    public List< SerializableKeyValue > KeyValues = new List<  SerializableKeyValue >();
}
