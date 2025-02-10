/*
 * 作者：阳贻凡
 */
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UIFramework
{
    //TODO 递归遍历，找到所有ui组件

    /// <summary>
    /// UI组件代码生成工具
    /// 目前只能找一个gameObj的所有组件
    /// </summary>
    public class GetUICompentTool: EditorWindow
    {
        //选择的UI界面
        private GameObject selectedGameObject;
        //所有组件
        private Component[] components;
        //选中的组件
        private bool[] componentSelected;

        [MenuItem("GameTools/UIFramework Tools/GetUICodeTool")]
        public static void ShowWindow()
        {
            GetWindow<GetUICompentTool>("UI组件代码生成工具");
        }

        private void OnGUI()
        {
            GUILayout.Label("选择一个GameObject", EditorStyles.boldLabel);
            selectedGameObject = (GameObject)EditorGUILayout.ObjectField("GameObject", selectedGameObject, typeof(GameObject), true);

            if (selectedGameObject != null)
            {
                if (components == null || components.Length == 0)
                {
                    components = selectedGameObject.GetComponents<Component>();
                    componentSelected = new bool[components.Length];
                }

                for (int i = 0; i < components.Length; i++)
                {
                    GUILayout.BeginHorizontal();
                    componentSelected[i] = EditorGUILayout.ToggleLeft(components[i].GetType().Name, componentSelected[i]);
                    GUILayout.EndHorizontal();
                }

                if (GUILayout.Button("生成代码"))
                {
                    GenerateCode();
                }
            }
        }

        //生成代码具体逻辑
        private void GenerateCode()
        {
            StringBuilder code = new StringBuilder();

            for (int i = 0; i < componentSelected.Length; i++)
            {
                if (componentSelected[i])
                {
                    code.AppendLine($"{components[i].GetType().Name} {GetVariableName(components[i].GetType().Name)}"
                        +$" = FindObjectOfType<{components[i].GetType().Name}>();");
                }
            }

            EditorGUIUtility.systemCopyBuffer = code.ToString();
            EditorUtility.DisplayDialog("代码生成成功", "代码已复制到剪贴板！", "确定");
        }

        private string GetVariableName(string typeName)
        {
            return char.ToLower(typeName[0]) + typeName.Substring(1);
        }
    }
}



