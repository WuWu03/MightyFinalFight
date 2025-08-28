using System;
using UnityEngine;

namespace GameFrameWork.Utils
{
    public class CommonUtil
    {
        /// <summary>
        /// 获取系统时间
        /// </summary>
        public static long GetSystemTime()
        {
            TimeSpan ts = new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0).Ticks);
            return (long)ts.TotalMilliseconds;
        }

        public static Transform FindChild(GameObject parent, string strName)
        {
            Transform[] allChildren = parent.GetComponentsInChildren<Transform>(true);

            foreach (Transform child in allChildren)
            {
                if (child.gameObject.name.Equals(strName))
                {
                    return child;
                }
            }

            return null;
        }

        public static T FindChildComponent<T>(GameObject parent, string strName) where T : Component
        {
            Transform[] allChildren = parent.GetComponentsInChildren<Transform>(true);

            foreach (Transform child in allChildren)
            {
                if (child.gameObject.name == strName)
                {
                    return child.gameObject.GetComponent<T>();
                }
            }

            return null;
        }

        public static void SetPos3(GameObject go, float x, float y, float z)
        {
            go.transform.position = new Vector3(x, y, z);
        }

        public static void SetRot3(GameObject go, float x, float y, float z)
        {
            go.transform.rotation = Quaternion.Euler(x, y, z);
        }

        public static void SetLocalPos3(GameObject go, float x, float y, float z)
        {
            go.transform.localPosition = new Vector3(x, y, z);
        }

        public static void SetLocalRot3(GameObject go, float x, float y, float z)
        {
            go.transform.localRotation = Quaternion.Euler(x, y, z);
        }

        public static void SetScale3(GameObject go, float x, float y, float z)
        {
            go.transform.localScale = new Vector3(x, y, z);
        }

        public static void SetTag(GameObject go, string tag, bool isSetChild = false)
        {
            if (isSetChild)
            {
                Transform[] childs = go.transform.GetComponentsInChildren<Transform>(true);
                for (int i = 0; i < childs.Length; i++)
                {
                    childs[i].tag = tag;
                }
            }

            else
            {
                go.tag = tag;
            }
        }

        public static bool CompareTo(double d1, double d2)
        {
            double difference = Math.Abs(d1 * 0.0001);
            return Math.Abs(d1 - d2) <= difference;
        }

        public static string ConvertLongToDateTime(long time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            DateTime dt = startTime.AddSeconds(time);
            return dt.ToString("yyyy/MM/dd HH:mm");
        }

        public static Vector2 ScreenPosToUGUIPos(Vector2 screenPos, RectTransform rectTrans, UnityEngine.Camera camera)
        {
            Vector2 resultPos = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTrans, screenPos, camera, out resultPos);

            return resultPos;
        }

        /// <summary>
        /// 根据权重随机一个元素
        /// </summary>
        /// <param name="weights"></param>
        /// <returns></returns>
        public static int RandomByWeight(int[] weights)
        {
            if (weights == null || weights.Length < 1)
            {
                return -1;
            }

            int sum = 0;

            for (int i = 0; i < weights.Length; i++)
            {
                sum += weights[i];
            }

            int random = UnityEngine.Random.Range(1, sum + 1);
            int sum_temp = 0;

            for (int i = 0; i < weights.Length; i++)
            {
                sum_temp += weights[i];
                if (random <= sum_temp)
                {
                    return i;
                }
            }

            return 0;
        }

        /// <summary>
        /// RGBA转16进制
        /// </summary>
        public static string RGBToHex(Color color)
        {
            byte r = (byte)(Mathf.Clamp01(color.r) * byte.MaxValue);
            byte g = (byte)(Mathf.Clamp01(color.g) * byte.MaxValue);
            byte b = (byte)(Mathf.Clamp01(color.b) * byte.MaxValue);
            byte a = (byte)(Mathf.Clamp01(color.a) * byte.MaxValue);

            return StringUtil.Append("#", ((r << 24) + (g << 16) + (b << 8) + a).ToString("X"));        }


        /// <summary>
        /// 16进制转RGBA
        /// </summary>
        public static Color HexToRGB(string hex)
        {
            int hexValue = Convert.ToInt32(hex.Trim().TrimStart('#').PadRight(8, 'F'), 16);

            float r = (byte)((hexValue >> 24) & 0xFF) / 255f;
            float g = (byte)((hexValue >> 16) & 0xFF) / 255f;
            float b = (byte)((hexValue >> 8) & 0xFF) / 255f;
            float a = (byte)(hexValue & 0xFF) / 255f;

            return new Color(r, g, b, a);
        }

        public static T[] AddElement<T>(T[] array, T newElement)
        {
            int length = array != null ? array.Length : 0;
            T[] newArray = new T[length + 1];

            if (array != null)
            {
                for (int i = 0; i < length; i++)
                {
                    newArray[i] = array[i];
                }
            }

            newArray[length] = newElement;
            return newArray;
        }

        public static T[] DeleteElement<T>(T[] array, params int[] deletePos)
        {
            if(array == null)
            {
                return null;
            }

            int length = array.Length - deletePos.Length;
            int pos = 0;
            T[] newArray = new T[length];

            for (int i = 0; i < array.Length; i++)
            {
                bool isDelete = false;

                for(int j = 0; j < deletePos.Length; j++)
                {
                    if(i == deletePos[j])
                    {
                        isDelete = true;
                        break;
                    }
                }

                if (!isDelete)
                {
                    newArray[pos] = array[i];
                    pos++;
                }
            }

            return newArray;
        }
    }
}