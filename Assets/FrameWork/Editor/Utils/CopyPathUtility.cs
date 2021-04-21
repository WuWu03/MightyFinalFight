using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;


namespace GameFrameWork.Editor
{
    public class CopyPathUtility
    {
        private static string[] components = new string[]
        {
            "Button",
            "MyButton",
            "Toggle",
            "Text",
            "Slider",
            "InputField",
            "ScrollRect",
            "GameObject"
        };

        private static string[] endPartten = new string[]//名字中包含此关键字的将作为本次搜索的根节点
        {
            "Panel",
        };

        /// <summary>
        /// 复制Lua路径
        /// </summary>
        [MenuItem("GameObject/CopyPath Lua", false, 0)]
        private static void CopyPathLua()
        {
            if (Selection.gameObjects == null || Selection.gameObjects.Length < 1)
            {
                Debug.Log("没选中任何物体!");
                return;
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                string nodePath = "";
                GetNodePath(Selection.gameObjects[i].transform, ref nodePath, Selection.gameObjects[i]);


                sb.Append("self." + Selection.gameObjects[i].name + " = " + "self:FindChild(\"");
                sb.Append(nodePath + "\")");

                for (int j = 0; j < components.Length; j++)
                {
                    if (Selection.gameObjects[i].GetComponent(components[j]) != null)
                    {
                        if (components[j].Equals("GameObject"))
                        {
                            sb.Append(".gameObject");
                        }
                        else
                        {
                            sb.Append(":GetComponent(\"" + components[j] + "\")");
                        }
                    }
                }
                sb.Append("\n");

            }

            Copy(sb.ToString());
        }

        /// <summary>
        /// 复制CSharp路径
        /// </summary>
        [MenuItem("GameObject/CopyPath CSharp &c", false, 0)]
        private static void CopyPathCSharp()
        {
            if (Selection.gameObjects == null || Selection.gameObjects.Length < 1)
            {
                Debug.Log("没选中任何物体!");
                return;
            }

            StringBuilder sb = new StringBuilder();
            List<string> defineList = new List<string>();
            List<string> variableList = new List<string>();

            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                string nodePath = "";
                GameObject go = Selection.gameObjects[i];
                Transform transform = Selection.gameObjects[i].transform;
                GetNodePath(transform, ref nodePath, go);

                bool isNone = true;

                for (int j = 0; j < components.Length; j++)
                {
                    if (go.GetComponent(components[j]) != null)
                    {
                        isNone = false;
                        defineList.Add("\t" + GetCSComponentPath(components[j], go.name));
                        variableList.Add("\t\t" + GetCSComponentPath(components[j], go.name, nodePath));
                    }
                }

                if (isNone)
                {
                    string componentName = go.GetComponent("Image") != null ? "Image" : "GameObject";
                    defineList.Add("\t" + GetCSComponentPath(componentName, go.name));
                    variableList.Add("\t\t" + GetCSComponentPath(componentName, go.name, nodePath));
                }
            }

            for (int i = 0; i < defineList.Count; i++)
            {
                sb.AppendLine(defineList[i]);
            }

            for (int i = 0; i < variableList.Count; i++)
            {
                sb.AppendLine(variableList[i]);
            }

            Copy(sb.ToString());
        }

        [MenuItem("Assets/CopyPath",false,2)]
        private static void CopyAssetsPath()
        {
            Copy(AssetDatabase.GetAssetPath(Selection.activeObject));
        }

        private static void Copy(string content)
        {
            TextEditor editor = new TextEditor();
            editor.text = content;
            editor.SelectAll();
            editor.Copy();
            EditorUtility.DisplayDialog("提示", "路径已复制到剪切板", "确定");
            Debug.Log("拷贝路径成功!");
        }

        private static string GetCSComponentPath(string componentName, string objName, string path = "")
        {
            string ret = string.Empty;
            string objScriptName = "m_" + objName;

            if (path.Equals(string.Empty))
            {
                ret = "private " + componentName + " " + objScriptName + " = null;\n";
            }
            else
            {
                if (!componentName.Equals("GameObject"))
                {
                    ret = objScriptName + " = transform.Find(\"" + path + "\").GetComponent<" + componentName + ">();\n";
                }
                else
                {
                    ret = objScriptName + " = transform.Find(\"" + path + "\").gameObject;\n";
                }
            }
            return ret;
        }

        private static void GetNodePath(Transform trans, ref string path, GameObject selectObj)
        {
            bool condition = trans != null;

            if (condition)
            {
                if (path == "")
                {
                    path = trans.name;
                }
                else //if (!trans.name.Equals(trans.root.name))
                {
                    path = trans.name + "/" + path;
                }
            }

            if (condition && trans.parent != null && trans.parent.gameObject != selectObj)
            {
                for (int i = 0; i < endPartten.Length; i++)
                {
                    if (trans.parent.name.Contains(endPartten[i]))
                    {
                        condition = false;
                        break;
                    }
                }
            }

            if (condition)
            {
                GetNodePath(trans.parent, ref path, selectObj);
            }
        }
    }
}