using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WuWuFramework
{
    public static class MethodExtend
    {
        /// <summary>
        /// 设置显示或隐藏，避免重复调用SetActive
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="value"></param>
        public static void SetActiveSelf(this GameObject gameObject, bool value)
        {
            if (gameObject is null || gameObject.activeSelf == value)
            {
                return;
            }

            gameObject.SetActive(value);
        }

        /// <summary>
        /// 设置显示或隐藏，避免重复调用SetActive
        /// </summary>
        /// <param name="component"></param>
        /// <param name="value"></param>

        public static void SetActiveSelf(this Component component, bool value)
        {
            if (component is null || component.gameObject is null)
            {
                return;
            }

            SetActiveSelf(component.gameObject, value);
        }


        /// <summary>
        /// 获取组件，如果没有则添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static T GetOrAddComponent<T>(this Transform transform) where T : Component
        {
            return GetOrAddComponent<T>(transform.gameObject);
        }


        /// <summary>
        /// 获取组件，如果没有则添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        /// <returns></returns>
        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            return GetOrAddComponent<T>(component.gameObject);
        }

        /// <summary>
        /// 获取组件，如果没有则添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 获取子物体的组件，如果没有则添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T GetOrAddComponentInChildren<T>(this GameObject go, string path) where T : Component
        {
            if (go is null || string.IsNullOrEmpty(path))
            {
                return null;
            }

            Transform child = go.transform.Find(path);

            if(child is null)
            {
                return null;
            }

            return GetOrAddComponent<T>(child.gameObject);
        }

        /// <summary>
        /// 获取父物体的组件，如果没有则返回null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static T GetComponentInParents<T>(this GameObject gameObject) where T : Component
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

        /// <summary>
        /// 设置物体的层级，如果isChild为true，则会设置所有子物体的层级
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="layer"></param>
        /// <param name="isChild"></param>
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

        /// <summary>
        /// 设置物体的Tag，如果isChild为true，则会设置所有子物体的Tag
        /// </summary>
        /// <param name="go"></param>
        /// <param name="tag"></param>
        /// <param name="isChild"></param>
        public static void SetTag(this GameObject go, string tag, bool isChild = false)
        {
            if (isChild)
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

        /// <summary>
        /// 限制文本的宽度
        /// </summary>
        /// <param name="text"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static void ClampTextWidth(this TextMeshProUGUI text, string str)
        {
            float height = text.rectTransform.sizeDelta.y;
            ClampTextWidth(text, str, height);
        }

        /// <summary>
        /// 限制文本的宽度
        /// </summary>
        /// <param name="text"></param>
        /// <param name="str"></param>
        /// <param name="height"></param>
        public static void ClampTextWidth(this TextMeshProUGUI text, string str, float height)
        {
            Vector2 size = text.GetPreferredValues(str);
            text.rectTransform.sizeDelta = new Vector2(size.x, height);
        }

        /// <summary>
        /// 限制文本的高度
        /// </summary>
        /// <param name="text"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static void ClampTextHeight(this TextMeshProUGUI text, string str)
        {
            float width = text.rectTransform.sizeDelta.x;
            ClampTextHeight(text, str, width);
        }

        /// <summary>
        /// 限制文本的高度
        /// </summary>
        /// <param name="text"></param>
        /// <param name="str"></param>
        /// <param name="width"></param>
        public static void ClampTextHeight(this TextMeshProUGUI text, string str, float width)
        {
            Vector2 size = text.GetPreferredValues(str);
            text.rectTransform.sizeDelta = new Vector2(width, size.y);
        }

        /// <summary>
        /// 重建布局，立即生效
        /// </summary>
        /// <param name="rectTransform"></param>
        public static void ForceRebuildLayoutImmediate(this RectTransform rectTransform)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }

        /// <summary>
        /// 获取ContentSizeFitter的尺寸
        /// </summary>
        /// <param name="contentSizeFitter"></param>
        /// <returns></returns>
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