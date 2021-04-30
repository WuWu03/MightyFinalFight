using System.IO;
using System.Text;
using UnityEngine;

namespace GameFrameWork
{
    public static class MethodExtend
    {
        public static void SetActive(this UnityEngine.Component go, bool value)
        {
            if (go == null)
            {
                return;
            }
            go.gameObject.SetActive(value);
        }

        public static T GetOrAddComponent<T>(this Transform transform) where T : UnityEngine.Component
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

        public static int[] ToIntArray(this string value, char partten = ',')
        {
            string[] valueStr = value.Split(partten);
            int[] ret = new int[valueStr.Length];

            for (int i = 0; i <= valueStr.Length/2; i++)
            {
                int result1 = 0;
                int result2 = 0;
                int.TryParse(valueStr[i], out result1);
                int.TryParse(valueStr[valueStr.Length - i - 1], out result2);

                ret[i] = result1;
                ret[valueStr.Length - i - 1] = result2;
            }
  
            return ret;
        }

        public static float ToFloat(this string value)
        {
            float result = 0;
            float.TryParse(value, out result);

            return result;
        }

        public static float[] ToFloatArray(this string value, char partten = ',')
        {
            string[] valueStr = value.Split(partten);
            float[] ret = new float[valueStr.Length];

            for (int i = 0; i <= valueStr.Length / 2; i++)
            {
                float result1 = 0;
                float result2 = 0;
                float.TryParse(valueStr[i], out result1);
                float.TryParse(valueStr[valueStr.Length - i - 1], out result2);

                ret[i] = result1;
                ret[valueStr.Length - i - 1] = result2;
            }

            return ret;
        }

        public static string[] ToStringArray(this string value,char partten = ',')
        {
            return value.Split(partten);
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

        public static Object GetAsset(this AssetBundle ab, string assetName)
        {
            string[] allAssetNames = ab.GetAllAssetNames();
            for (int i = 0; i < allAssetNames.Length; i++)
            {
                string assetNameTemp = Path.GetFileNameWithoutExtension(allAssetNames[i]);
                if (assetNameTemp.Equals(assetName))
                {
                    return ab.LoadAsset(allAssetNames[i]);
                }
            }

            return null;
        }

        public static Object GetAsset(this AssetBundle ab, string assetName, System.Type type)
        {
            string[] allAssetNames = ab.GetAllAssetNames();
            for (int i = 0; i < allAssetNames.Length; i++)
            {
                string assetNameTemp = Path.GetFileNameWithoutExtension(allAssetNames[i]);
                if (assetNameTemp.Equals(assetName))
                {
                    return ab.LoadAsset(allAssetNames[i], type);
                }
            }

            return null;
        }

        public static StringBuilder AppendInt(this StringBuilder sb, int n, int len = 0)
        {
            int l;
            int k;
            if (n == 0)
                l = 0;
            else
                l = (int)System.Math.Floor(System.Math.Log10(n < 0 ? -n : n));
            if (len - 1 > l)
                l = len - 1;
            k = (int)System.Math.Round(System.Math.Pow(10, l));

            do
            {
                if (n < 0)
                {
                    sb.Append('-');
                    n = -n;
                }
                else
                {
                    sb.Append((char)('0' + n / k));
                    n %= k;
                    k /= 10;
                }
            } while (k > 0);

            return sb;
        }
    }
}