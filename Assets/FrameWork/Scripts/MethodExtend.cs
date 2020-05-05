using UnityEngine;

namespace FrameWork
{
    public static class MethodExtend
    {
        public static void SetActive(this Component go, bool value)
        {
            if (go == null)
            {
                return;
            }
            go.gameObject.SetActive(value);
        }

        public static T GetOrAddComponent<T>(this Transform transform) where T : Component
        {
            return GetOrAddComponent<T>(transform.gameObject);
        }

        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            T ret = go.GetComponent<T>();

            if (ret == null)
            {
                ret = go.AddComponent<T>();
            }

            return ret;
        }

        public static void SetLayer(this GameObject gameObject, string layer, bool isChild = true)
        {
            gameObject.layer = LayerMask.NameToLayer(layer);

            if (isChild)
            {
                Transform[] children = gameObject.transform.GetComponentsInChildren<Transform>(true);
                for (int i = 1; i < children.Length; i++)
                {
                    children[i].gameObject.layer = LayerMask.NameToLayer(layer);
                }
            }
        }

        public static T FindComponentInParents<T>(this GameObject gameObject) where T : Component
        {
            if (gameObject == null) return null;

            T comp = gameObject.GetComponent<T>();

            if (comp == null)
            {
                Transform t = gameObject.transform.parent;

                while (t != null && comp == null)
                {
                    comp = t.gameObject.GetComponent<T>();
                    t = t.parent;
                }
            }

            return comp;
        }

        public static int ToInt(this string value)
        {
            int result = 0;
            int.TryParse(value, out result);

            return result;
        }

        public static float ToFloat(this string value)
        {
            float result = 0;
            float.TryParse(value, out result);

            return result;
        }

        public static bool ToBoolean(this string value)
        {
            bool result = false;
            bool.TryParse(value, out result);

            return result;
        }

        public static LitJson.JsonData ToJson(this string value)
        {
            return LitJson.JsonMapper.ToObject(value);
        }

        public static Object GetMainAsset(this AssetBundle ab)
        {
            if (ab.GetAllAssetNames().Length > 0)
            {
                return ab.LoadAsset(ab.GetAllAssetNames()[0]);
            }
            return null;
        }

        public static Object GetMainAsset(this AssetBundle ab, System.Type type)
        {
            if (ab.GetAllAssetNames().Length > 0)
            {
                return ab.LoadAsset(ab.GetAllAssetNames()[0], type);
            }
            return null;
        }
    }
}