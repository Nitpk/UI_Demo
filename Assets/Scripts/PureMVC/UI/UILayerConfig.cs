/*
 * 作者：阳贻凡
 */
using UnityEngine;

namespace MVC_UIFramework
{
    /// <summary>
    /// 层级配置信息
    /// </summary>
    [System.Serializable]
    public class UILayerConfig
    {
        [Tooltip("UI层级类型")]
        public E_UILayerType layerType;
        [Tooltip("基础排序值")]
        public int baseOrder = 1000;
        [Tooltip("是否开启深度管理")]
        public bool enableDepth = true;
        [Tooltip("层级最大深度")]
        public int maxDepth = 50;
        [Tooltip("深度步长")]
        public int depthStep = 10;
        [Tooltip("渲染模式")]
        public RenderMode renderType = RenderMode.ScreenSpaceOverlay;
    }
}