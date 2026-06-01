using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace WuWuFramework
{
    [CustomPropertyDrawer(typeof(EnumLabelAttribute))]
    public class EnumLabelDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SetUpCustomEnumNames(property, property.enumNames);

            if (property.propertyType == SerializedPropertyType.Enum)
            {
                EditorGUI.BeginChangeCheck();
                string[] displayedOptions = property.enumNames
                        .Where(enumName => m_CustomEnumNames.ContainsKey(enumName))
                        .Select<string, string>(enumName => m_CustomEnumNames[enumName])
                        .ToArray();

                int[] indexArray = GetIndexArray(((EnumLabelAttribute)attribute).orders);

                if (indexArray.Length != displayedOptions.Length)
                {
                    indexArray = new int[displayedOptions.Length];
                    for (int i = 0; i < indexArray.Length; i++)
                    {
                        indexArray[i] = i;
                    }
                }

                string[] items = new string[displayedOptions.Length];
                items[0] = displayedOptions[0];

                for (int i = 0; i < displayedOptions.Length; i++)
                {
                    items[i] = displayedOptions[indexArray[i]];
                }

                int index = -1;

                for (int i = 0; i < indexArray.Length; i++)
                {
                    if (indexArray[i] == property.enumValueIndex)
                    {
                        index = i;
                        break;
                    }
                }

                if ((index == -1) && (property.enumValueIndex != -1)) 
                { 
                    SortingError(position, property, label); 
                    return; 
                }

                index = EditorGUI.Popup(position, ((EnumLabelAttribute)attribute).label, index, items);

                if (EditorGUI.EndChangeCheck())
                {
                    if (index >= 0)
                    {
                        property.enumValueIndex = indexArray[index];
                    }
                }
            }
        }

        public void SetUpCustomEnumNames(SerializedProperty property, string[] enumNames)
        {
            object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(EnumLabelAttribute), false);

            foreach (EnumLabelAttribute customAttribute in customAttributes)
            {
                Type enumType = fieldInfo.FieldType;

                foreach (string enumName in enumNames)
                {
                    FieldInfo field = enumType.GetField(enumName);

                    if (field == null)
                    {
                        continue;
                    }

                    EnumLabelAttribute[] attrs = (EnumLabelAttribute[])field.GetCustomAttributes(customAttribute.GetType(), false);

                    if (!m_CustomEnumNames.ContainsKey(enumName))
                    {
                        foreach (EnumLabelAttribute labelAttribute in attrs)
                        {
                            m_CustomEnumNames.Add(enumName, labelAttribute.label);
                        }
                    }
                }
            }
        }

        private int[] GetIndexArray(int[] order)
        {
            int[] indexArray = new int[order.Length];

            for (int i = 0; i < order.Length; i++)
            {
                int index = 0;
                for (int j = 0; j < order.Length; j++)
                {
                    if (order[i] > order[j])
                    {
                        index++;
                    }
                }
                indexArray[i] = index;
            }

            return indexArray;
        }

        private void SortingError(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, new GUIContent(label.text + " (sorting error)"));
            EditorGUI.EndProperty();
        }

        private readonly Dictionary<string, string> m_CustomEnumNames = new Dictionary<string, string>();
    }
}