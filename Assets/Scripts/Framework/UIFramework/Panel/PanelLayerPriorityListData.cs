/*
 * 作者：阳贻凡
 */
using System;
using UnityEngine;

namespace UIFramework
{
    /// <summary>
    /// 面板层内部层级
    /// </summary>
    [Serializable]
    public struct PanelLayerPriorityListData
    {
        [SerializeField]
        [Tooltip("面板层级优先级")]
        private E_PanelLayerPriority layerPriority;
        [SerializeField]
        [Tooltip("层级对象")]
        private Transform layerTrans;

        /// <summary>
        /// 层级对象
        /// </summary>
        public Transform LayerTrans
        {
            get { return layerTrans; }
            set { layerTrans = value; }
        }

        /// <summary>
        /// 面板层内部层级优先级
        /// </summary>
        public E_PanelLayerPriority LayerPriority
        {
            get { return layerPriority; }
            set { layerPriority = value; }
        }

        public PanelLayerPriorityListData(E_PanelLayerPriority prio, Transform layerT)
        {
            layerPriority = prio;
            layerTrans = layerT;
        }

    }
}