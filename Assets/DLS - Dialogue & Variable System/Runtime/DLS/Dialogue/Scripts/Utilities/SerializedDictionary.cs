using System;
using System.Collections.Generic;
using UnityEngine;

namespace DLS.Dialogue.Utilities
{
    [Serializable]
    public class SerializedDictionary<TKey, TValue> : ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<TKey> keys = new();

        [SerializeField]
        private List<TValue> values = new();

        private Dictionary<TKey, TValue> dictionary = new();

        public Dictionary<TKey, TValue> ToDictionary()
        {
            return dictionary;
        }

        public void FromDictionary(Dictionary<TKey, TValue> dict)
        {
            dictionary = dict;
            keys.Clear();
            values.Clear();

            foreach (var kvp in dict)
            {
                keys.Add(kvp.Key);
                values.Add(kvp.Value);
            }
        }

        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();

            foreach (var kvp in dictionary)
            {
                keys.Add(kvp.Key);
                values.Add(kvp.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            dictionary = new Dictionary<TKey, TValue>();

            if (keys.Count != values.Count)
            {
                Debug.LogError($"The number of keys ({keys.Count}) does not match the number of values ({values.Count}) in the serialized dictionary.");
                return;
            }

            for (int i = 0; i < keys.Count; i++)
            {
                if (!dictionary.ContainsKey(keys[i]))
                {
                    dictionary.Add(keys[i], values[i]);
                }
            }
        }
    }
}