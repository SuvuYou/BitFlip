using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UglySerializableDictionary<TKey, TValue>
{
    [System.Serializable]
    public struct KeyValuePairSerializable
    {
        public TKey key;
        public TValue value;

        public KeyValuePairSerializable(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }
    }

    [SerializeField] private List<KeyValuePairSerializable> items = new();

    public Dictionary<TKey, TValue> ToDictionary()
    {
        Dictionary<TKey, TValue> dictionary = new();
        foreach (var item in items)
        {
            dictionary[item.key] = item.value;
        }
        return dictionary;
    }

    public void FromDictionary(Dictionary<TKey, TValue> dictionary)
    {
        items.Clear();
        foreach (var kvp in dictionary)
        {
            items.Add(new KeyValuePairSerializable(kvp.Key, kvp.Value));
        }
    }
}
