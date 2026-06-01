using System;
using System.Collections.Generic;

namespace WuWuFramework.Serialize
{
    public static class PlayerPrefs
    {
        public static string GetPlayerPrefsSaveKey()
        {
            return UnityEngine.PlayerPrefs.GetString(playerPrefsSaveKey, string.Empty);
        }

        public static List<string> GetPlayerPrefsSaveKeyList()
        {
            string playerPrefsSave = UnityEngine.PlayerPrefs.GetString(playerPrefsSaveKey, string.Empty);

            if(!string.IsNullOrEmpty(playerPrefsSave))
            {
                List<string> playerPrefsSaveKeyList = new();
                string[] keys = playerPrefsSave.Split('_');
                playerPrefsSaveKeyList.AddRange(keys);

                return playerPrefsSaveKeyList;
            }

            return null;
        }

        public static void SetInt(string key, int value)
        {
            UnityEngine.PlayerPrefs.SetInt(key, value);
#if UNITY_EDITOR
            AddSaveKey(key);
#endif
        }

        public static void SetString(string key, string value)
        {
            UnityEngine.PlayerPrefs.SetString(key, value);
#if UNITY_EDITOR
            AddSaveKey(key);
#endif
        }

        public static void SetFloat(string key, float value)
        {
            UnityEngine.PlayerPrefs.SetFloat(key, value);
#if UNITY_EDITOR
            AddSaveKey(key);
#endif
        }

        public static void DeleteKey(string key)
        {
            UnityEngine.PlayerPrefs.DeleteKey(key);
#if UNITY_EDITOR
            DeleteSaveKey(key);
#endif
        }

        public static void DeleteAll()
        {
            UnityEngine.PlayerPrefs.DeleteAll();
#if UNITY_EDITOR
            UnityEngine.PlayerPrefs.DeleteKey(playerPrefsSaveKey);
#endif
        }

        private static void AddSaveKey(string key)
        {
            string playerPrefsSave = UnityEngine.PlayerPrefs.GetString(playerPrefsSaveKey, string.Empty);
            bool isNullOrEmpty = string.IsNullOrEmpty(playerPrefsSave);

            if (isNullOrEmpty || !playerPrefsSave.Contains(key))
            {
                playerPrefsSave += isNullOrEmpty ? key : "_" + key;
                UnityEngine.PlayerPrefs.SetString(playerPrefsSaveKey, playerPrefsSave);
            }
        }

        private static void DeleteSaveKey(string key)
        {
            string playerPrefsSave = UnityEngine.PlayerPrefs.GetString(playerPrefsSaveKey, string.Empty);
            
            if (!string.IsNullOrEmpty(playerPrefsSave))
            {
                int index = playerPrefsSave.IndexOf(key, StringComparison.Ordinal);

                if (index == 0)
                {
                    playerPrefsSave = playerPrefsSave.Remove(0, key.Length);
                }
                else if (index > 0)
                {
                    playerPrefsSave = playerPrefsSave.Remove(index - 1, key.Length + 1);
                }
                else
                {
                    return;
                }

                UnityEngine.PlayerPrefs.SetString(playerPrefsSaveKey, playerPrefsSave);
            }
        }

        private const string playerPrefsSaveKey = "WuWuFramework_PlayerPrefs_Save_Key";
    }
}
