/*
 * 作者：阳贻凡
 */
using System;
using UnityEngine;
using UnityEngine.UI;

namespace UIFramework
{
    /// <summary>
    /// UI管理器
    /// </summary>
    public class UIMgr : MonoBehaviour
    {
        [Tooltip("是否在Awake时自动初始化")]
        [SerializeField]
        private bool initOnAwake = true;

        //层级，当添加新的自定义层级时往这里加入
        private PanelLayer panelLayer;
        private WindowLayer windowLayer;

        
        private Canvas mainCanvas;
        private GraphicRaycaster graphicRaycaster;
        /// <summary>
        /// 主Canvas
        /// </summary>
        public Canvas MainCanvas
        { 
            get 
            {
                if (mainCanvas == null)
                {
                    mainCanvas = GetComponent<Canvas>();
                }
                return mainCanvas; 
            } 
        }
        /// <summary>
        /// 主相机
        /// </summary>
        public Camera UICamera 
        { 
            get{ return MainCanvas.worldCamera; } 
        }

        private void Awake()
        {
            if (initOnAwake)
            {
                Init();
            }
        }
        private void OnDestroy()
        {
            //移除监听
            if(windowLayer != null)
            {
                windowLayer.animStartPlay -= OnUIAnimStart;
                windowLayer.allAnimOverPlay -= OnAllUIAnimOver;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            
            //初始化面板层级
            if (panelLayer == null)
            {
                panelLayer = gameObject.GetComponentInChildren<PanelLayer>(true);
                if (panelLayer == null)
                {
                    Debug.LogError("[UIFramework]: 找不到面板层级");
                }
                else
                {
                    panelLayer.Initialize();
                }
            }
            //初始化窗口层级
            if (windowLayer == null)
            {
                windowLayer = gameObject.GetComponentInChildren<WindowLayer>(true);
                if (windowLayer == null)
                {
                    Debug.LogError("[UIFramework]: 找不到窗口层级");
                }
                else
                {
                    windowLayer.Initialize();
                    windowLayer.animStartPlay += OnUIAnimStart;
                    windowLayer.allAnimOverPlay += OnAllUIAnimOver;
                }
            }
            //初始化射线检测
            graphicRaycaster = MainCanvas.GetComponent<GraphicRaycaster>();
        }
        #region 面板层级相关API
        /// <summary>
        /// 显示面板
        /// </summary>
        public void ShowPanel(string uiName)
        {
            panelLayer.ShowUIByName(uiName);
        }

        /// <summary>
        /// 显示面板并且传递属性参数
        /// </summary>
        public void ShowPanel<T>(string uiName, T properties) where T : IPanelProperties
        {
            panelLayer.ShowUIByName<T>(uiName, properties);
        }

        /// <summary>
        /// 隐藏面板
        /// </summary>
        public void HidePanel(string uiName)
        {
            panelLayer.HideUIByName(uiName);
        }
        /// <summary>
        /// 注册面板
        /// </summary>
        public void RegisterPanel<TPanel>(string uiName, TPanel controller) where TPanel : IPanelController
        {
            panelLayer.RegisterUI(uiName, controller);
        }
        /// <summary>
        /// 注销面板
        /// </summary>
        public void UnregisterPanel<TPanel>(string uiName, TPanel controller) where TPanel : IPanelController
        {
            panelLayer.UnregisterUI(uiName, controller);
        }
        /// <summary>
        /// 判断面板是否打开中
        /// </summary>
        public bool IsPanelOpen(string uiName)
        {
            return panelLayer.IsPanelVisible(uiName);
        }
        /// <summary>
        /// 隐藏所有面板层的界面
        /// </summary>
        public void HideAllPanels(bool animate = true)
        {
            panelLayer.HideAll(animate);
        }
        #endregion

        #region 窗口层级相关API
        /// <summary>
        /// 打开窗口
        /// </summary>
        /// <param name="uiName">UI名</param>
        public void OpenWindow(string uiName)
        {
            windowLayer.ShowUIByName(uiName);
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="uiName">UI名</param>
        public void CloseWindow(string uiName)
        {
            windowLayer.HideUIByName(uiName);
        }

        /// <summary>
        /// 关闭当前的窗口
        /// </summary>
        public void CloseCurrentWindow()
        {
            if (windowLayer.CurrentWindow != null)
            {
                CloseWindow(windowLayer.CurrentWindow.UIName);
            }
        }

        /// <summary>
        /// 打开窗口并且传递属性参数
        /// </summary>
        /// <param name="uiName">UI名</param>
        /// <param name="properties">属性</param>
        public void OpenWindow<T>(string uiName, T properties) where T : IWindowProperties
        {
            windowLayer.ShowUIByName<T>(uiName, properties);
        }

        /// <summary>
        /// 注册窗口
        /// </summary>
        public void RegisterWindow<T>(string uiName, T controller) where T : IWindowController
        {
            windowLayer.RegisterUI(uiName, controller);
        }

        /// <summary>
        /// 注销窗口
        /// </summary>
        public void UnregisterWindow<T>(string uiName, T controller) where T : IWindowController
        {
            windowLayer.UnregisterUI(uiName, controller);
        }
        /// <summary>
        /// 隐藏所有窗口
        /// </summary>
        /// <param name="animate">是否播放动画</param>
        public void CloseAllWindows(bool animate = true)
        {
            windowLayer.HideAll(animate);
        }
        #endregion
        /// <summary>
        /// 注册UI
        /// </summary>
        /// <param name="uiName">UI名</param>
        /// <param name="controller">UI</param>
        /// <param name="uiTransform">UI位置组件</param>
        public void RegisterUI(string uiName,IController controller,Transform uiTransform = null)
        {
            if (controller == null) return;

            
            if (controller is IPanelController)
            {
                RegisterPanel(uiName, controller as IPanelController);
                if (uiTransform != null)
                {
                    panelLayer.SetUIParent(controller, uiTransform);
                }
            }
            else if (controller is IWindowController)
            {
                RegisterWindow(uiName, controller as IWindowController);
                if (uiTransform != null)
                {
                    windowLayer.SetUIParent(controller, uiTransform);
                }
            }


        }
        /// <summary>
        /// 检查界面是否被注册过了
        /// </summary>
        public bool IsRegistered(string uiName)
        {
            return panelLayer.IsRegistered(uiName) 
                || windowLayer.IsRegistered(uiName);
        }

        /// <summary>
        /// 跟上面一样，只不过多了个类型的返回
        /// </summary>
        public bool IsScreenRegistered(string screenId, out Type type)
        {
            if (panelLayer.IsRegistered(screenId))
            {
                type = typeof(IPanelController);
                return true;
            }
            else if (windowLayer.IsRegistered(screenId))
            {
                type = typeof(IWindowController);
                return true;
            }

            type = null;
            return false;
        }

        /// <summary>
        /// 隐藏所有界面
        /// </summary>
        /// <param name="animate">是否播放动画</param>
        public void HideAll(bool animate = true)
        {
            HideAllPanels(animate);
            CloseAllWindows(animate);
        }

        //当有UI动画在播放时
        private void OnUIAnimStart()
        {
            //关闭射线检测
            if (graphicRaycaster != null)
            {
                graphicRaycaster.enabled = false;
            }
        }
        //当所有UI动画结束播放时
        private void OnAllUIAnimOver()
        {
            //开启射线检测
            if (graphicRaycaster != null)
            {
                graphicRaycaster.enabled = true;
            }
        }
    }
}