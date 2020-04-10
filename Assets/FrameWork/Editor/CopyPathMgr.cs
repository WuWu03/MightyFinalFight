using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Text;
using UnityEngine.UI;
using System;

public class CopyPathMgr : Editor
{
    static string[] components = new string[]
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

    [MenuItem("GameObject/CopyPath Lua", false, 0)]
    static void GenerateLua()
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

            //复制到剪贴板
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
        TextEditor editor = new TextEditor();
        editor.text = sb.ToString();
        editor.SelectAll();
        editor.Copy();
        Debug.Log("拷贝路径成功!");
    }

    [MenuItem("GameObject/CopyPath CSharp &c", false, 0)]
    static void CopyPathCSharp()
    {
        if (Selection.gameObjects == null || Selection.gameObjects.Length < 1)
        {
            Debug.Log("没选中任何物体!");
            return;
        }

        bool isGenerate = true;

        if (Selection.gameObjects.Length > 1)
        {
            isGenerate = false;
        }
        else
        {
            isGenerate = !Selection.gameObjects[0].name.Contains("_");
        }

        if(isGenerate)
        {
            GenerateCS(Selection.gameObjects[0]);
        }
        else 
        {
            CopyCS(Selection.gameObjects);
        }
    }

    static void CopyCS(GameObject[] goArray)
    {
        StringBuilder sb = new StringBuilder();
        List<string> variableList = new List<string>();
        List<string> defineList = new List<string>();

        for (int goIndex = 0; goIndex < goArray.Length; goIndex++)
        {
            GameObject go = goArray[goIndex];
            GetCSList(go, variableList, defineList, 0, 1);
        }

        for (int i = 0; i < variableList.Count; i++)
        {
            if (!string.IsNullOrEmpty(variableList[i]))
            {
                sb.Append(variableList[i]);
            }
        }

        for (int i = 0; i < defineList.Count; i++)
        {
            if (!string.IsNullOrEmpty(defineList[i]))
            {
                sb.Append(defineList[i]);
            }
        }

        GenerateEnd(sb);
    }
    static void GenerateCS(GameObject go)
    {
        string filePath = Application.dataPath + "/Scripts/UI/" + go.name + ".cs";
        bool isItem = go.name.Contains("Item");
        bool canGenerate = !File.Exists(filePath) && !isItem;

        StringBuilder sb = new StringBuilder();
        List<string> variableList = new List<string>();
        List<string> defineList = new List<string>();

        if (canGenerate)
        {
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int day = DateTime.Now.Month;
            int hour = DateTime.Now.Hour;
            int minute = DateTime.Now.Minute;

            sb.AppendLine("/*******************************************************");
            sb.AppendFormat("**{0}-{1}-{2} {3}:{4}**************************************\n", year,month,day,hour,minute);
            sb.AppendLine("**Create By GQY*****************************************");
            sb.AppendLine("*******************************************************/");
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("using UnityEngine.UI;");
            sb.AppendLine("using DG.Tweening;");
            sb.AppendLine("");
            sb.AppendFormat("public class {0} : PanelBase<{0}>", go.name);
            sb.AppendLine("\n{");
            sb.AppendLine("\tprotected override string PanelName");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\t get {return \"" + go.name + "\";}");
            sb.AppendLine("\t}\n");
            sb.AppendLine("\tprotected override void OnInit()");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tPanelLayer = UIDefine.UILayer.PanelLayer;");
            sb.AppendLine("\t}\n");
        }

        GetCSList(go, variableList, defineList, 1, -1, !isItem, "Item");

        if (isItem)
        {
            sb.Append("\tpublic override void CreateHandle()\n\t{\n");
        }
        else
        {
            sb.Append("\tprotected override void OnLoadViewCallback()\n\t{\n");
        }
       
        for (int i = 0; i < variableList.Count; i++)
        {
            if (!string.IsNullOrEmpty(variableList[i]))
            {
                sb.Append(variableList[i]);
            }
        }

        sb.Append("\t}\n");

        if (canGenerate)
        {
            sb.AppendLine("\tprotected override void OnAfterOpenHandle()");
            sb.AppendLine("\t{");
            sb.AppendLine("\t}\n");
            sb.AppendLine("\tprotected override void OnBeforeCloseHandle()");
            sb.AppendLine("\t{");
            sb.AppendLine("\t}\n");
            sb.AppendLine("\tprotected override void OnClose()");
            sb.AppendLine("\t{");
            sb.AppendLine("\t}\n");
            sb.AppendLine("\tprotected override void OnDestroy()");
            sb.AppendLine("\t{");
            sb.AppendLine("\t}");
            sb.Append("}");
        }

        for (int i = 0; i < defineList.Count; i++)
        {
            if (!string.IsNullOrEmpty(defineList[i]))
            {
                sb.Append(defineList[i]);
            }
        }

        GenerateEnd(sb, true, filePath);
    }

    static void GetCSList(GameObject go,List<string> variableList,List<string> defineList,int startIndex = 0,int endIndex = -1,bool condition = false,params string[] partten)
    {
        Transform[] trans = go.GetComponentsInChildren<Transform>();
        endIndex = endIndex == -1 ? trans.Length : endIndex;

        for (int i = startIndex; i < endIndex; i++)
        {
            if (trans[i].name.Contains("_"))
            {
                string nodePath = string.Empty;
                GetNodePath(trans[i], ref nodePath, go);

                if (condition)
                {
                    for (int j = 0; j < partten.Length; j++)
                    {
                        if (nodePath.Contains(partten[j])) continue;
                    }
                }

                bool isNone = true;

                for (int j = 0; j < components.Length; j++)
                {
                    if (trans[i].GetComponent(components[j]) != null)
                    {
                        isNone = false;
                        defineList.Add("\t" + GetComponentPath(components[j], trans[i].name));
                        variableList.Add("\t\t" + GetComponentPath(components[j], trans[i].name, nodePath));
                    }
                }

                if (isNone)
                {
                    string componentName = trans[i].GetComponent("Image") != null ? "Image" : "GameObject";
                    defineList.Add("\t" + GetComponentPath(componentName, trans[i].name));
                    variableList.Add("\t\t" + GetComponentPath(componentName, trans[i].name, nodePath));
                }
            }
        }
    }
    static string GetComponentPath(string componentName, string objName, string path = "")
    {
        string ret = string.Empty;
        int objFirstName = objName.ToCharArray()[1] - 32;
        string objScriptName = "m" + objName.Replace(objName[1], (char)objFirstName);

        if (path.Equals(string.Empty))
        {
            ret = "private " + componentName + " " + objScriptName + " = null;\n";
        }
        else
        {
            if(!componentName.Equals("GameObject"))
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

    static string[] pathPartten = new string[2] 
    {
        "Panel",
        "Item",
    };
    static void GetNodePath(Transform trans, ref string path, GameObject selectObj)
    {
        if (path == "")
        {
            path = trans.name;
        }
        else if (!trans.name.Equals(trans.root.name))
        {
            path = trans.name + "/" + path;
        }

        bool condition = true;
        if (trans.parent != null && trans.parent.gameObject != selectObj)
        {
            for (int i = 0; i < pathPartten.Length; i++)
            {
                if (trans.parent.name.Contains(pathPartten[i]))
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

    static void GenerateEnd(StringBuilder sb,bool wirteToFile = false,string filePath = null)
    {
        if (wirteToFile)
        {
            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
            AssetDatabase.Refresh();
        }
        else
        {
            TextEditor editor = new TextEditor();
            editor.text = sb.ToString();
            editor.SelectAll();
            editor.Copy();
            Debug.Log("已复制到剪切板");
        }
    }
}
