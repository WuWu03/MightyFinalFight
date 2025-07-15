using GameFrameWork.Pool;
using GameFrameWork.UI;
using GameFrameWork.Utils;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace GameFrameWork
{
    public static class MethodExtend
    {
        public static void SetActiveSelf (this GameObject gameObject,bool value)
        {
            if (gameObject == null || gameObject.activeSelf == value)
            {
                return;
            }

            gameObject.SetActive(value);
        }

        public static void SetActiveSelf(this UnityEngine.Component component, bool value)
        {
            if (component == null)
            {
                return;
            }

            component.gameObject.SetActiveSelf(value);
        }

        public static T GetOrAddComponent<T>(this Transform transform) where T : UnityEngine.Component
        {
            return GetOrAddComponent<T>(transform.gameObject);
        }

        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            T result = go.GetComponent<T>();

            if (result == null)
            {
                result = go.AddComponent<T>();
            }

            return result;
        }

        public static T GetOrAddComponentInChildren<T>(this GameObject go,string path) where T : Component
        {
            T result = go.GetComponentInChildren<T>(true);

            if (result == null)
            {
                Transform child = go.transform.Find(path);

                if(child != null)
                {
                    result = child.gameObject.AddComponent<T>();
                }
            }

            return result;
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
            if (gameObject == null)
            {
                return null;
            }

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

        public static void SetSprite(this Image renderer, string spriteName)
        {
            if (renderer == null || string.IsNullOrEmpty(spriteName))
            {
                return;
            }

            string sritePath = PathUtil.FormatPath(PathUtil.GetUIAtlasPath(), spriteName);

            AssetsPool.instance.Get<Sprite>(sritePath, (string assetPath, UnityEngine.Object obj, object[] param) =>
            {
                renderer.sprite = obj as Sprite;
            });
        }

        public static void PutSprite(this Image renderer,string spriteName)
        {
            if (renderer == null || renderer.sprite == null)
            {
                return;
            }

            Sprite sprite = renderer.sprite;
            renderer.sprite = null;
            string spritePath = PathUtil.FormatPath(PathUtil.GetUIAtlasPath(), spriteName);
            AssetsPool.instance.Put(spritePath, sprite);
        }

        public static int ToInt(this string value)
        {
            if (!string.IsNullOrEmpty(value) && int.TryParse(value, out int result))
            {
                return result;
            }

            return 0;
        }

        public static long ToLong(this string value)
        {
            if (!string.IsNullOrEmpty(value) && long.TryParse(value, out long result))
            {
                return result;
            }

            return 0;
        }

        public static float ToFloat(this string value)
        {
            if (!string.IsNullOrEmpty(value) && float.TryParse(value, out float result))
            {
                return result;
            }

            return 0;
        }

        public static double ToDouble(this string value)
        {
            if (!string.IsNullOrEmpty(value) && double.TryParse(value, out double result))
            {
                return result;
            }

            return 0;
        }

        public static bool ToBool(this string value)
        {
            if (!string.IsNullOrEmpty(value) && bool.TryParse(value, out bool result))
            {
                return result;
            }

            return false;
        }

        public static int[] ToIntArray(this string value, char partten = ',')
        {
            if(string.IsNullOrEmpty(value))
            {
                return null;
            }

            string[] valueStr = value.Split(partten);
            int[] result = new int[valueStr.Length];

            for (int i = 0; i <= valueStr.Length / 2; i++)
            {
                if (int.TryParse(valueStr[i], out int result1))
                {
                    result[i] = result1;
                }

                if (int.TryParse(valueStr[valueStr.Length - i - 1], out int result2))
                {
                    result[valueStr.Length - i - 1] = result2;
                }
            }

            return result;
        }

        public static long[] ToLongArray(this string value, char partten = ',')
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            string[] valueStr = value.Split(partten);
            long[] result = new long[valueStr.Length];

            for (int i = 0; i <= valueStr.Length / 2; i++)
            {
                if (long.TryParse(valueStr[i], out long result1))
                {
                    result[i] = result1;
                }

                if (long.TryParse(valueStr[valueStr.Length - i - 1], out long result2))
                {
                    result[valueStr.Length - i - 1] = result2;
                }
            }

            return result;
        }

        public static float[] ToFloatArray(this string value, char partten = ',')
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            string[] valueStr = value.Split(partten);
            float[] result = new float[valueStr.Length];

            for (int i = 0; i <= valueStr.Length / 2; i++)
            {
                if (float.TryParse(valueStr[i], out float result1))
                {
                    result[i] = result1;
                }

                if (float.TryParse(valueStr[valueStr.Length - i - 1], out float result2))
                {
                    result[valueStr.Length - i - 1] = result2;
                }
            }

            return result;
        }

        public static double[] ToDoubleArray(this string value, char partten = ',')
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            string[] valueStr = value.Split(partten);
            double[] result = new double[valueStr.Length];

            for (int i = 0; i <= valueStr.Length / 2; i++)
            {
                if (double.TryParse(valueStr[i], out double result1))
                {
                    result[i] = result1;
                }

                if (double.TryParse(valueStr[valueStr.Length - i - 1], out double result2))
                {
                    result[valueStr.Length - i - 1] = result2;
                }
            }

            return result;
        }

        public static bool[] ToBoolArray(this string value, char partten = ',')
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            string[] valueStr = value.Split(partten);
            bool[] result = new bool[valueStr.Length];

            for (int i = 0; i <= valueStr.Length / 2; i++)
            {
                if (bool.TryParse(valueStr[i], out bool result1))
                {
                    result[i] = result1;
                }

                if (bool.TryParse(valueStr[valueStr.Length - i - 1], out bool result2))
                {
                    result[valueStr.Length - i - 1] = result2;
                }
            }

            return result;
        }

        public static string[] ToStringArray(this string value, char partten = ',')
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            return value.Split(partten);
        }

        public static Vector2 ToVector2(this string value, char partten = ',')
        {
            float x = 0;
            float y = 0;

            if (!string.IsNullOrEmpty(value))
            {
                string[] valueStr = value.Split(partten);

                if (valueStr.Length == 2)
                {
                    x = float.Parse(valueStr[0]);
                    y = float.Parse(valueStr[1]);
                }
            }

            return new Vector2(x, y);
        }

        public static Vector3 ToVector3(this string value, char partten = ',')
        {
            float x = 0;
            float y = 0;
            float z = 0;

            if (!string.IsNullOrEmpty(value))
            {
                string[] valueStr = value.Split(partten);

                if (valueStr.Length == 3)
                {
                    x = float.Parse(valueStr[0]);
                    y = float.Parse(valueStr[1]);
                    z = float.Parse(valueStr[2]);
                }
            }
            return new Vector3(x, y, z);
        }

        public static void ForceRebuildLayoutImmediate(this RectTransform rectTransform)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }

        public static Vector2 GetContentSizeFitterPreferredSize(this ContentSizeFitter contentSizeFitter)
        {
            RectTransform rectTransform = contentSizeFitter.GetComponent<RectTransform>();
            rectTransform.ForceRebuildLayoutImmediate();

            float horizontalSize;
            float verticalSize;

            if (contentSizeFitter.horizontalFit == ContentSizeFitter.FitMode.MinSize)
            {
                horizontalSize = LayoutUtility.GetMinSize(rectTransform, 0);
            }
            else
            {
                horizontalSize = LayoutUtility.GetPreferredSize(rectTransform, 0);
            }

            if (contentSizeFitter.verticalFit == ContentSizeFitter.FitMode.MinSize)
            {
                verticalSize = LayoutUtility.GetMinSize(rectTransform, 1);
            }
            else
            {
                verticalSize = LayoutUtility.GetPreferredSize(rectTransform, 1);
            }

            return new Vector2(horizontalSize, verticalSize);
        }


        public static void AppendInt(this StringBuilder sb, int n, int len = 0)
        {
            int l;
            int k;

            if (n == 0)
            {
                l = 0;
            }
            else
            {
                l = (int)System.Math.Floor(System.Math.Log10(n < 0 ? -n : n));
            }

            if (len - 1 > l)
            {
                l = len - 1;
            }

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
        }

        public static object CreatePanelParam(this IPanel panel, string paramName)
        {
            return CreatePanelParam(panel.GetType().Name, paramName);
        }

        private static object CreatePanelParam(string panelTypeName, string paramName)
        {
            string panelParamName = StringUtil.Append(panelTypeName, paramName);
            Type panelViewType = Type.GetType(panelParamName);

            if (panelViewType == null)
            {
                Log.LogError("[", panelParamName, "] 不存在");
                return null;
            }

            if (panelViewType != null)
            {
                object panelParam = Activator.CreateInstance(panelViewType);
                return panelParam;
            }

            return null;
        }
    }
}