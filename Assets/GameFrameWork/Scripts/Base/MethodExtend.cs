using UnityEngine;
using UnityEngine.UI;

namespace GameFrameWork
{
    public static class MethodExtend
    {
        public static void SetActiveSelf(this GameObject gameObject, bool value)
        {
            if (gameObject is null || gameObject.activeSelf == value)
            {
                return;
            }

            gameObject.SetActive(value);
        }

        public static void SetActiveSelf(this Component component, bool value)
        {
            if (component is null)
            {
                return;
            }

            component.gameObject.SetActiveSelf(value);
        }

        public static T GetOrAddComponent<T>(this Transform transform) where T : Component
        {
            return GetOrAddComponent<T>(transform.gameObject);
        }

        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            return GetOrAddComponent<T>(component.gameObject);
        }

        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            if (go is null)
            {
                return null;
            }

            if (!go.TryGetComponent(out T result))
            {
                result = go.AddComponent<T>();
            }

            return result;
        }

        public static T GetOrAddComponentInChildren<T>(this GameObject go, string path) where T : Component
        {
            T result = go.GetComponentInChildren<T>(true);

            if (result == null)
            {
                Transform child = go.transform.Find(path);

                if (child != null)
                {
                    result = child.gameObject.AddComponent<T>();
                }
            }

            return result;
        }

        public static void SetLayer(this GameObject gameObject, string layer, bool isChild = true)
        {
            if (string.IsNullOrEmpty(layer))
            {
                return;
            }

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
            if (gameObject is null)
            {
                return null;
            }

            if (!gameObject.TryGetComponent(out T component))
            {
                Transform parent = gameObject.transform.parent;

                while (parent is not null && component is null)
                {
                    component = parent.gameObject.GetComponent<T>();
                    parent = parent.parent;
                }
            }

            return component;
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

        public static ulong ToULong(this string value)
        {
            if (!string.IsNullOrEmpty(value) && ulong.TryParse(value, out ulong result))
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

        public static int[] ToIntArray(this string value, char separator = ',')
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            string[] valueStr = value.Split(separator);
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

        public static long[] ToLongArray(this string value, char separator = ',')
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            string[] valueStr = value.Split(separator);
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

        public static float[] ToFloatArray(this string value, char separator = ',')
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            string[] valueStr = value.Split(separator);
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

        public static double[] ToDoubleArray(this string value, char separator = ',')
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            string[] valueStr = value.Split(separator);
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

        public static bool[] ToBoolArray(this string value, char separator = ',')
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            string[] valueStr = value.Split(separator);
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

        public static string[] ToStringArray(this string value, char separator = ',')
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            return value.Split(separator);
        }

        public static Vector2 ToVector2(this string value, char separator = ',')
        {
            float x = 0;
            float y = 0;

            if (!string.IsNullOrEmpty(value))
            {
                string[] valueStr = value.Split(separator);

                if (valueStr.Length == 2)
                {
                    x = float.Parse(valueStr[0]);
                    y = float.Parse(valueStr[1]);
                }
            }

            return new Vector2(x, y);
        }

        public static Vector3 ToVector3(this string value, char separator = ',')
        {
            float x = 0;
            float y = 0;
            float z = 0;

            if (!string.IsNullOrEmpty(value))
            {
                string[] valueStr = value.Split(separator);

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

        public static Vector2 GetContentSizeFitterSize(this ContentSizeFitter contentSizeFitter)
        {
            RectTransform rectTransform = contentSizeFitter.GetComponent<RectTransform>();
            rectTransform.ForceRebuildLayoutImmediate();

            float horizontalSize = contentSizeFitter.horizontalFit switch
            {
                ContentSizeFitter.FitMode.MinSize => LayoutUtility.GetMinSize(rectTransform, 0),
                ContentSizeFitter.FitMode.PreferredSize => LayoutUtility.GetPreferredSize(rectTransform, 0),
                _ => 0,
            };

            float verticalSize = contentSizeFitter.verticalFit switch
            {
                ContentSizeFitter.FitMode.MinSize => LayoutUtility.GetMinSize(rectTransform, 1),
                ContentSizeFitter.FitMode.PreferredSize => LayoutUtility.GetPreferredSize(rectTransform, 1),
                _ => 0,
            };
            
            return new Vector2(horizontalSize, verticalSize);
        }
    }
}