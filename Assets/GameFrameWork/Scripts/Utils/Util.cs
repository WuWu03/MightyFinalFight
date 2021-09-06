using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System;
using System.Reflection;
using UnityEngine.Networking;
using UnityEditor;
/// <summary>
/// 游戏里常用的实用工具
/// </summary>

namespace GameFrameWork.Utility
{
    public class Util
    {
        /// <summary>
        /// http下载
        /// </summary>
        /// <returns></returns>
        public static void WebRequest(MonoBehaviour mb, string url, GameFrameWorkAction<UnityWebRequest> call, GameFrameWorkAction<string> error, GameFrameWorkAction<float> progressCall = null)
        {
            mb.StartCoroutine(StartUnityWebRequest(url, call, error, progressCall));
        }

        //uwr下载
        private static IEnumerator StartUnityWebRequest(string url, GameFrameWorkAction<UnityWebRequest> call, GameFrameWorkAction<string> error, GameFrameWorkAction<float> progressCall)
        {
            UnityWebRequest uwr = UnityWebRequest.Get(url);
            uwr.timeout = 5;
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                if (error != null) error(uwr.error);
            }
            else
            {
                while (!uwr.isDone)
                {
                    if (progressCall != null) progressCall(uwr.downloadProgress);
                    yield return null;
                }

                if (uwr.isDone)
                {
                    if (call != null) call(uwr);
                }
            }
        }

        public static Vector3 HexagonXToWorldPos(Vector2Int hexagonPos, float scaleX, float scaleY)
        {
            return new Vector3(hexagonPos.x * scaleX, hexagonPos.y * scaleY, 0);
        }

        /// <summary>
        /// 四边形坐标转六边形坐标
        /// </summary>
        public static Vector2Int ToHexagonXPos(int x, int y)
        {
            return new Vector2Int(2 * x + y % 2, y);
        }

        /// <summary>
        /// 六边形坐标转四边形坐标
        /// </summary>>
        public static Vector2Int To4Pos(int x, int y)
        {
            return new Vector2Int((x - y % 2) / 2, y);
        }

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

            else go.tag = tag;
        }

        public static void SetLayer(GameObject go, int layer, bool isSetChild = false)
        {
            if (isSetChild)
            {
                Transform[] childs = go.transform.GetComponentsInChildren<Transform>(true);
                for (int i = 0; i < childs.Length; i++)
                {
                    childs[i].gameObject.layer = layer;
                }
            }

            else go.layer = layer;
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

        public static int RandomByWeight(int[] weights)
        {
            if(weights == null || weights.Length < 1)
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

            return -1;
        }

        public static string ToRGBHex(Color c)
        {
            if (c == default(Color))
            {
                c = Color.black;
            }

            byte r = (byte)(Mathf.Clamp01(c.r) * 255);
            byte g = (byte)(Mathf.Clamp01(c.g) * 255);
            byte b = (byte)(Mathf.Clamp01(c.b) * 255);
            byte a = (byte)(Mathf.Clamp01(c.a) * 255);

            return TextUtil.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", r, g, b, a);
        }

        public static T[] AddElement<T>(T[] array, T newElement)
        {
            int length = array.Length;
            T[] tempArray = new T[length];

            for (int i = 0; i < length; i++)
            {
                tempArray[i] = array[i];
            }

            T[] newArray = new T[length + 1];

            for (int i = 0; i < length; i++)
            {
                newArray[i] = tempArray[i];
            }

            newArray[length] = newElement;
            
            return newArray;
        }

        public static T[] DeleteElement<T>(T[] array, params int[] deletePos)
        {
            int length = array.Length;
            T[] tempArray = new T[length];

            for (int i = 0; i < length; i++)
            {
                tempArray[i] = array[i];
            }

            length -= deletePos.Length;
            T[] newArray = new T[length];

            int index = 0;
            int currPos = deletePos[index];

            for (int i = 0; i < length; i++)
            {   
                if (i + index >= currPos)
                {
                    index += 1;
                    currPos = deletePos[index];
                }

                newArray[i] = tempArray[i + index];
            }

            return newArray;
        }

        //某点是否在多边形内
        public static bool PolygonContainsPoint(Vector2Int[] polyPoints, Vector2Int p)
        {
            var j = polyPoints.Length - 1;
            var inside = false;
            for (int i = 0; i < polyPoints.Length; j = i++)
            {
                var pi = polyPoints[i];
                var pj = polyPoints[j];
                if (((pi.y >= p.y && p.y > pj.y) || (pj.y >= p.y && p.y > pi.y)) &&
                    (p.x < (pj.x - pi.x) * (p.y - pi.y) / (pj.y - pi.y) + pi.x))
                    inside = !inside;
            }
            return inside;
        }

        public static Vector2Int[] PolygonRandomPoints(Vector2Int[] polyPoints, int maxCount = 1)
        {
            List<Vector2Int> result = new List<Vector2Int>();
            Vector2Int centerPT = Vector2Int.zero;

            for (int i = 0; i < polyPoints.Length; i++)
            {
                centerPT += polyPoints[i];
            }

            centerPT /= polyPoints.Length;

            for (int i = 0; i < polyPoints.Length; i++)
            {
                int count = 0;
                int index1 = i;
                int index2 = i + 1;

                if (i == polyPoints.Length - 1)
                {
                    index1 = 0;
                    index2 = polyPoints.Length - 1;
                }

                while (count < maxCount)
                {
                    Vector2Int ab = polyPoints[index1] - centerPT;
                    Vector2Int ac = polyPoints[index2] - centerPT;

                    float x = UnityEngine.Random.Range(0, 1);
                    float y = UnityEngine.Random.Range(0, 1);
                    float x1 = 0;
                    float y1 = 0;

                    if (x + y > 10)
                    {
                        x1 = 1 - x;
                        y1 = 1 - y;
                    }
                    else
                    {
                        x1 = x;
                        y1 = y;
                    }

                    int abx = Mathf.RoundToInt((float)ab.x * x1);
                    int aby = Mathf.RoundToInt((float)ab.y * x1);

                    int acx = Mathf.RoundToInt((float)ac.x * y1);
                    int acy = Mathf.RoundToInt((float)ac.y * y1);

                    Vector2Int pt = centerPT + new Vector2Int(abx, aby) + new Vector2Int(acx, acy);
                    result.Add(pt);
                    count++;
                }
            }

            return result.ToArray();
        }
    }
}