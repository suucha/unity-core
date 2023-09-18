using Newtonsoft.Json;
using System;
using UnityEngine;

namespace SuuchaStudio.Unity.Core.Storages
{
    public class UnityLocalStorage : SuuchaBase, ILocalStorage
    {
        public T Get<T>(string key)
        {
            if (!PlayerPrefs.HasKey(key))
            {
                return default;
            }
            var json = PlayerPrefs.GetString(key);
            if (string.IsNullOrEmpty(json))
            {
                return default;
            }
            try
            {
                var value = JsonConvert.DeserializeObject<T>(json);
                return value;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Local storage error, key:{key}, json:{json}\n error message:{ex.Message}");
            }
            return default;
        }

        public float GetFloat(string key)
        {
            return GetFloat(key, 0);
        }

        public float GetFloat(string key, float defaultValue)
        {
            if (!PlayerPrefs.HasKey(key))
            {
                return defaultValue;
            }
            var value = PlayerPrefs.GetFloat(key);
            return value;
        }

        public int GetInt(string key)
        {
            return GetInt(key, 0);
        }

        public int GetInt(string key, int defaultValue)
        {
            if (!PlayerPrefs.HasKey(key))
            {
                return defaultValue;
            }
            var value = PlayerPrefs.GetInt(key);
            return value;
        }

        public long GetLong(string key)
        {
            return GetLong(key, 0L);
        }

        public long GetLong(string key, long defaultValue)
        {
            var highKey = $"HHHHH{key}";
            var lowKey = $"LLLLL{key}";
            if (!PlayerPrefs.HasKey(highKey) || !PlayerPrefs.HasKey(lowKey))
            {
                return defaultValue;
            }
            var highValue = (long)PlayerPrefs.GetInt(highKey);
            var lowValue = (long)PlayerPrefs.GetInt(lowKey);
            var value = (highValue << 32) + lowValue;
            return value;
        }

        public string GetString(string key)
        {

            return GetString(key, null);
        }

        public string GetString(string key, string defaultValue)
        {
            if (!PlayerPrefs.HasKey(key))
            {
                return defaultValue;
            }
            var value = PlayerPrefs.GetString(key);
            return value;
        }

        public void Save()
        {
            PlayerPrefs.Save();
        }

        public void Set<T>(string key, T value)
        {
            PlayerPrefs.SetString(key, JsonConvert.SerializeObject(value));
        }

        public void SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }

        public void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        public void SetLong(string key, long value)
        {
            int lowValue = (int)(value & 0x000000ffffffffL);
            int highValue = (int)(value >> 32);
            var highKey = $"HHHHH{key}";
            var lowKey = $"LLLLL{key}";
            PlayerPrefs.SetInt(highKey, highValue);
            PlayerPrefs.SetInt(lowKey, lowValue);
        }

        public void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }
    }
}
