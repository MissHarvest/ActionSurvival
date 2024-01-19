using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class SerializableDictionary<TKey, TValue>
{
    public List<TKey> keys;
    public List<TValue> values;

    public SerializableDictionary() { }

    public SerializableDictionary(Dictionary<TKey, TValue> dict) => FromDictionary(dict);

    public Dictionary<TKey, TValue> ToDictionary()
    {
        var dict = new Dictionary<TKey, TValue>();
        for (int i = 0; i < keys.Count; i++)
            dict.TryAdd(keys[i], values[i]);
        return dict;
    }

    public void FromDictionary(Dictionary<TKey, TValue> dict)
    {
        keys = dict.Keys.ToList();
        values = dict.Values.ToList();
    }
}