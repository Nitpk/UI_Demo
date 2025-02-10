/*
 * 作者：阳贻凡
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIFramework
{
    /// <summary>
    /// UI设置模板
    /// </summary>
    [CreateAssetMenu(fileName = "UISettings",menuName = "UIFramework/UI Settings")]
    public class UISettings : ScriptableObject
    {
        [Header("UIMgr的预制体")]
        [SerializeField] 
        private UIMgr UIMgrPrefab = null;
        [Header("UI注册列表")]
        [SerializeField] 
        private List<GameObject> UIToRegisterList = null;
        [Header("实例化UI时是否设置失活")]
        [SerializeField] 
        private bool deactivateUI = true;

        /// <summary>
        /// 创建UIMgr对象
        /// </summary>
        /// <param name="instanceAndRegisterUI">预先实例化并且注册UI</param>
        public UIMgr CreateUIInstance(bool instanceAndRegisterUI = true)
        {
            
            UIMgr newUIMgr = Instantiate(UIMgrPrefab);
            
            if (instanceAndRegisterUI)
            {
                foreach (var ui in UIToRegisterList)
                {
                    var uiInstance = Instantiate(ui);
                    var uiController = uiInstance.GetComponent<IController>();

                    if (uiController != null)
                    {
                        newUIMgr.RegisterUI(ui.name, uiController, uiInstance.transform);
                        if (deactivateUI && uiInstance.activeSelf)
                        {
                            uiInstance.SetActive(false);
                        }
                    }
                    else
                    {
                        Debug.LogError(string.Format("[UIFramework]: [{0}]界面没有Controller",  ui.name));
                    }
                }
            }

            return newUIMgr;
        }

        private void OnValidate()
        {
            //调整UI注册表时立刻检查是否合法

            List<GameObject> objectsToRemove = new List<GameObject>();
            foreach (var obj in UIToRegisterList)
            {
                IController uiController = obj.GetComponent<IController>();
                if (uiController == null)
                {
                    objectsToRemove.Add(obj);
                }
            }

            if (objectsToRemove.Count > 0)
            {
                foreach (var obj in objectsToRemove)
                {
                    Debug.LogError(string.Format("[UIFramework]: 移除[{0}]界面", obj.name));
                    UIToRegisterList.Remove(obj);
                }
            }
        }
    }
}