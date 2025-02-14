/*
 * 作者：阳贻凡
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MVC_UIFramework
{
    /// <summary>
    /// UI层级管理器
    /// </summary>
    public class UILayerMgr : MonoBehaviour
    {
        [Header("层级配置")]
        [SerializeField]
        private List<UILayerConfig> layerConfigs = new();
        private readonly Dictionary<E_UILayerType, LayerData> layers = new();

        /// <summary>
        /// 层级数据
        /// </summary>
        private class LayerData
        {
            /// <summary>
            /// 层级父节点
            /// </summary>
            public Transform container;
            /// <summary>
            /// 当前深度指针
            /// </summary>
            public int currentDepth;
            /// <summary>
            /// 层级的窗口对象列表
            /// </summary>
            public List<GameObject> windows = new();
            /// <summary>
            /// 层级配置信息
            /// </summary>
            public UILayerConfig config;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            DontDestroyOnLoad(gameObject);
            InitializeLayers();
        }
        //初始化层级
        private void InitializeLayers()
        {
            //层级父物体
            GameObject layersRoot = new("UILayers");
            layersRoot.transform.SetParent(transform);

            //通过配置信息配置
            foreach (var config in layerConfigs)
            {
                var layerGo = new GameObject(config.layerType.ToString());
                layerGo.transform.SetParent(layersRoot.transform);

                var canvas = layerGo.AddComponent<Canvas>();

                canvas.renderMode = config.renderType;

                if(config.renderType!=RenderMode.ScreenSpaceOverlay)
                    canvas.worldCamera = transform.Find("UIMainCamera").GetComponent<Camera>();

                canvas.vertexColorAlwaysGammaSpace = true;
                canvas.overrideSorting = true;
                canvas.sortingOrder = config.baseOrder;

                layerGo.AddComponent<GraphicRaycaster>();
                var canvasScaler =layerGo.AddComponent<AdaptCanvasScaler>();
                canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                //layerGo.AddComponent<SafeAreaAdapter>();

                layers[config.layerType] = new LayerData
                {
                    container = layerGo.transform,
                    config = config,
                    currentDepth = 0
                };
            }
        }
        /// <summary>
        /// 将UI添加到层级
        /// </summary>
        public void AddToLayer(GameObject uiObject, E_UILayerType layerType)
        {
            if (!layers.TryGetValue(layerType, out var layerData))
            {
                Debug.LogError($"未配置的层级类型: {layerType}");
                return;
            }

            uiObject.transform.SetParent(layerData.container, false);
            InitializeUIObject(uiObject, layerData);
        }
        //初始化UI
        private void InitializeUIObject(GameObject uiObject, LayerData layerData)
        {
            var canvas = uiObject.AddComponentIfMissing<Canvas>();
            var raycaster = uiObject.AddComponentIfMissing<GraphicRaycaster>();
            uiObject.AddComponent<CanvasGroup>();

            if (layerData.config.enableDepth)
            {
                canvas.overrideSorting = true;
                UpdateDepth(uiObject, layerData);
            }

        }
        //更新UI深度
        private void UpdateDepth(GameObject uiObject, LayerData layerData)
        {
            if (layerData.currentDepth >= layerData.config.maxDepth)
                RebalanceLayer(layerData);

            var canvas = uiObject.GetComponent<Canvas>();
            canvas.sortingOrder = layerData.config.baseOrder + layerData.currentDepth;
            layerData.currentDepth += layerData .config.depthStep;
            layerData.windows.Add(uiObject);
        }
        //重新平衡该层级的所有UI深度
        private void RebalanceLayer(LayerData layerData)
        {
            layerData.currentDepth = 0;
            foreach (var window in layerData.windows)
            {
                var canvas = window.GetComponent<Canvas>();
                canvas.sortingOrder = layerData.config.baseOrder + layerData.currentDepth;
                layerData.currentDepth += layerData.config.depthStep;
            }
        }
    }
}