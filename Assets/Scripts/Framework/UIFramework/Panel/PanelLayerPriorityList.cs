/*
 * 作者：阳贻凡
 */
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UIFramework
{
    /// <summary>
    /// 面板层内部层级列表
    /// </summary>
    [Serializable]
    public class PanelLayerPriorityList
    {
        [SerializeField]
        [Tooltip("存储layer的GameObject，渲染优先级由这些GameObject在层级结构中的顺序来决定")]
        private List<PanelLayerPriorityListData> layerList = null;
        //层级字典
        private Dictionary<E_PanelLayerPriority, Transform> layerDic;
        /// <summary>
        /// 层级字典
        /// </summary>
        public Dictionary<E_PanelLayerPriority, Transform> LayerDic
        {
            get{return layerDic;}
        }

        public PanelLayerPriorityList(List<PanelLayerPriorityListData> datas)
        {
            layerList = datas;
            layerDic = new Dictionary<E_PanelLayerPriority, Transform>();
            for (int i = 0; i < layerList.Count; i++)
            {
                layerDic.Add(layerList[i].LayerPriority, layerList[i].LayerTrans);
            }
        }

    }
}