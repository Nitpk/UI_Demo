/*
 * 作者：阳贻凡
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MVC_UIFramework
{
    /// <summary>
    /// UI配置
    /// </summary>
    [CreateAssetMenu(fileName = "UISettings", menuName = "MVC_UIFramework/Create UISettings")]
    public class UISettings : ScriptableObject
    {
        [System.Serializable]
        public class UIEntry
        {
            [Tooltip("UI名（与Mediator相同）")]
            public MediatorNames uiKey;
            [Tooltip("UI预制体")]
            public GameObject prefab;
            [Tooltip("UI所属层级")]
            public E_UILayerType layerType;
        }

        [Header("UI层级预制体")]
        public GameObject uiLayerMgr;

        [Header("UI注册表")]
        public List<UIEntry> entries = new();

        [Header("实例化UI时是否设置失活")]
        public bool inactiveUIOnInit;
    }
}