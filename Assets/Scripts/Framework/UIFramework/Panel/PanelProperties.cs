/*
 * 作者：阳贻凡
 */
using UnityEngine;

namespace UIFramework
{
    /// <summary>
    /// 面板属性
    /// </summary>
    [System.Serializable]
    public class PanelProperties : IPanelProperties
    {
        [SerializeField]
        [Tooltip("面板层内部层级")]
        private E_PanelLayerPriority layerPriority;

        /// <summary>
        /// 面板层内部层级优先级
        /// </summary>
        public E_PanelLayerPriority PanelLayerPriority
        {
            get { return layerPriority; }
            set { layerPriority = value; }
        }
    }
}