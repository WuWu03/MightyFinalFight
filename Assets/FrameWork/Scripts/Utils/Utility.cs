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

namespace GameFrameWork.Utils
{
    public class Utility
    {
        /// <summary>
        /// http下载
        /// </summary>
        /// <returns></returns>
        public static void WebRequest(MonoBehaviour mb, string url, VoidParamT<UnityWebRequest> call, VoidParamT<string> error, VoidParamT<float> progressCall = null)
        {
            mb.StartCoroutine(StartUnityWebRequest(url, call, error, progressCall));
        }

        //uwr下载
        private static IEnumerator StartUnityWebRequest(string url, VoidParamT<UnityWebRequest> call, VoidParamT<string> error, VoidParamT<float> progressCall)
        {
            UnityWebRequest uwr = UnityWebRequest.Get(url);
            uwr.timeout = 5;
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
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
        /// 判断是否在数组范围内
        /// </summary>
        public static bool IsSafetyArray<T>(T[] arrays, int index)
        {
            return arrays != null && index >= 0 && index < arrays.Length;
        }
        /// <summary>
        /// 判断是否在数组范围内
        /// </summary>
        public static bool IsSafetyArray<T>(IList<T> arrays, int index)
        {
            return arrays != null && index >= 0 && index < arrays.Count;
        }

        /// <summary>
        /// 计算字符串的MD5值
        /// </summary>
        public static string MD5(string source)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(source);
            byte[] md5Data = md5.ComputeHash(data, 0, data.Length);
            md5.Clear();

            string destString = "";
            for (int i = 0; i < md5Data.Length; i++)
            {
                destString += System.Convert.ToString(md5Data[i], 16).PadLeft(2, '0');
            }
            destString = destString.PadLeft(32, '0');
            return destString;
        }

        /// <summary>
        /// 计算文件的MD5值
        /// </summary>
        public static string MD5File(string file)
        {
            try
            {
                FileStream fs = new FileStream(file, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(fs);
                fs.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("md5file() fail, error:" + ex.Message);
            }
        }

        /// <summary>
        /// 获取系统时间
        /// </summary>
        public static long GetSystemTime()
        {
            TimeSpan ts = new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0).Ticks);
            return (long)ts.TotalMilliseconds;
        }

        /// <summary>
        /// 根据类名，获取Type
        /// </summary>
        public static System.Type GetType(string typeName)
        {
            Type type = Type.GetType(typeName);

            if (type != null)
            {
                return type;
            }

            string assemblyName = typeName;
            if (typeName.Contains("."))
            {
                assemblyName = typeName.Substring(0, typeName.IndexOf('.'));
            }


            Assembly assembly = null;

            try
            {
                assembly = Assembly.Load(assemblyName);
            }
            catch (Exception)
            {

            }

            if (assembly == null)
            {
                return null;
            }

            return assembly.GetType(typeName);
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

        public static void SetCamToDev(UnityEngine.Camera cam)
        {
            float screenHeight = Screen.height;

            Log.Debugger.Log("screenHeight = " + screenHeight);

            float orthographicSize = cam.orthographicSize;

            float aspectRatio = Screen.width * 1.0f / Screen.height;

            float cameraWidth = orthographicSize * 2 * aspectRatio;

            Log.Debugger.Log("cameraWidth = " + cameraWidth);

            //if ( cameraWidth < CommonDefine.DevCamWidth )
            //{
            //    orthographicSize = CommonDefine.DevCamWidth / (2 * aspectRatio);
            //    Debug.Log("new orthographicSize = " + orthographicSize);
            //    cam.orthographicSize = orthographicSize;
            //}
        }

        public Vector2 ScreenPosToUGUIPos(Vector2 screenPos, RectTransform rectTrans, UnityEngine.Camera camera)
        {
            Vector2 resultPos = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTrans, screenPos, camera, out resultPos);

            return resultPos;
        }

        public static void CreateConfigData<T, P>(string name, string ext,string dir = null)
            where T : BaseScriptableObject<P>
            where P : BaseConfigData
        {
            string directory = Application.dataPath + "/ConfigData/";
            if (!string.IsNullOrEmpty(dir)) directory = dir;

            string fileName = directory + name + ext;
            if (File.Exists(fileName))
            {
                return;
            }

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            T data = ScriptableObject.CreateInstance<T>();
            //AssetDatabase.CreateAsset(data, directory.Substring(directory.IndexOf("Assets")) + name + ext);
            //AssetDatabase.SaveAssets();
            //AssetDatabase.Refresh();
        }
    }
}