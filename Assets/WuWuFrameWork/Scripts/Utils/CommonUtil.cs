using System;
using UnityEngine;

namespace WuWuFramework.Utils
{
    public static class CommonUtil
    {
        /// <summary>
        /// 比较两个double类型的数值是否相等，允许一定的误差范围
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static bool CompareTo(this double d1, double d2)
        {
            double difference = 1.0E-9;
            return Math.Abs(d1 - d2) <= difference;
        }

        /// <summary>
        /// 获取当前时间
        /// </summary>
        public static long GetCurrTime()
        {
            return DateTime.Now.Ticks;
        }

        /// <summary>
        /// 获取当前UTC时间
        /// </summary>
        public static long GetCurrUTCTime()
        {
            return DateTime.UtcNow.Ticks;
        }

        /// <summary>
        /// 将给定时间戳转为UTC时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime ConvertToUTCTime(long timeStamp)
        {
            DateTime origin = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddMilliseconds(timeStamp);
        }

        /// <summary>
        /// 将给定时间戳转为指定时区的时间
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <param name="zoneId"></param>
        /// <returns></returns>
        public static DateTime ConvertToTimeZone(long timeStamp, int zoneId)
        {
            TimeZoneInfo timeZone = TimeZoneInfo.GetSystemTimeZones()[zoneId];
            DateTime time = TimeZoneInfo.ConvertTimeFromUtc(ConvertToUTCTime(timeStamp), timeZone);
            return time;
        }

        /// <summary>
        /// 屏幕坐标转UGUI坐标
        /// </summary>
        /// <param name="screenPos"></param>
        /// <param name="rectTrans"></param>
        /// <param name="camera"></param>
        /// <returns></returns>
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

            return StringUtil.Append("#", ((r << 24) + (g << 16) + (b << 8) + a).ToString("X"));
        }


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

        /// <summary>
        /// 向数组中添加一个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="newElement"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 移除数组中指定位置的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="deletePos"></param>
        /// <returns></returns>
        public static T[] DeleteElement<T>(T[] array, params int[] deletePos)
        {
            if (array == null)
            {
                return null;
            }

            int length = array.Length - deletePos.Length;
            int pos = 0;
            T[] newArray = new T[length];

            for (int i = 0; i < array.Length; i++)
            {
                bool isDelete = false;

                for (int j = 0; j < deletePos.Length; j++)
                {
                    if (i == deletePos[j])
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