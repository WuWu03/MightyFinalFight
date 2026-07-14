using System;
using System.Collections.Generic;

namespace WuWuFramework.Serialize
{
    public static class PlayerPrefs
    {
        private const string SAVE_KEY_NAME = "WuWuFramework_PlayerPrefs_Save_Key";

        public static string GetSaveKeyStr()
        {
            return GetString(SAVE_KEY_NAME);
        }

        public static List<string> GetSaveKeys()
        {
            string playerPrefsSave = GetString(SAVE_KEY_NAME);

            if (!string.IsNullOrEmpty(playerPrefsSave))
            {
                List<string> playerPrefsSaveKeys = new();
                string[] keys = playerPrefsSave.Split("@WuWu@");
                playerPrefsSaveKeys.AddRange(keys);
                return playerPrefsSaveKeys;
            }

            return null;
        }

        public static int GetInt(string key)
        {
            return string.IsNullOrEmpty(key) ? 0 : UnityEngine.PlayerPrefs.GetInt(key, 0);
        }

        public static void SetInt(string key, int value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new WuWuFrameworkException("PlayerPrefs SetInt Key值为空");
            }

            UnityEngine.PlayerPrefs.SetInt(key, value);
#if UNITY_EDITOR
            AddSaveKey(key);
#endif
        }

        public static string GetString(string key)
        {
            return string.IsNullOrEmpty(key) ? string.Empty : UnityEngine.PlayerPrefs.GetString(key, string.Empty);
        }

        public static void SetString(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new WuWuFrameworkException("PlayerPrefs SetString Key值为空");
            }

            UnityEngine.PlayerPrefs.SetString(key, value);
#if UNITY_EDITOR
            AddSaveKey(key);
#endif
        }

        public static float GetFloat(string key)
        {
            return string.IsNullOrEmpty(key) ? 0 : UnityEngine.PlayerPrefs.GetFloat(key, 0);
        }

        public static void SetFloat(string key, float value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new WuWuFrameworkException("PlayerPrefs SetFloat Key值为空");
            }

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
            UnityEngine.PlayerPrefs.DeleteKey(SAVE_KEY_NAME);
#endif
        }

        private static void AddSaveKey(string key)
        {
            string playerPrefsSave = UnityEngine.PlayerPrefs.GetString(SAVE_KEY_NAME, string.Empty);
            bool isNullOrEmpty = string.IsNullOrEmpty(playerPrefsSave);

            if (isNullOrEmpty || !playerPrefsSave.Contains(key))
            {
                playerPrefsSave += isNullOrEmpty ? key : "@WuWu@" + key;
                UnityEngine.PlayerPrefs.SetString(SAVE_KEY_NAME, playerPrefsSave);
            }
        }

        private static void DeleteSaveKey(string key)
        {
            string playerPrefsSave = UnityEngine.PlayerPrefs.GetString(SAVE_KEY_NAME, string.Empty);

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

                UnityEngine.PlayerPrefs.SetString(SAVE_KEY_NAME, playerPrefsSave);
            }
        }
    }
}
