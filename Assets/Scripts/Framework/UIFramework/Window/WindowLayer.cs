/*
 * 作者：阳贻凡
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIFramework
{
    /// <summary>
    /// 窗口层级
    /// </summary>
    public class WindowLayer : UILayer<IWindowController>
    {
        [Tooltip("弹窗层级")]
        [SerializeField]
        private WindowPopLayer popLayer = null;
        /// <summary>
        /// 当前窗口
        /// </summary>
        public IWindowController CurrentWindow { get;private set; }
        //窗口队列
        private Queue<WindowQueueData> windowQue;
        //窗口历史记录
        private Stack<WindowQueueData> windowHistory;

        /// <summary>
        /// 当有窗口动画开始播放时执行
        /// </summary>
        public event Action animStartPlay;
        /// <summary>
        /// 当没有任何窗口动画正在播放时执行
        /// </summary>
        public event Action allAnimOverPlay;
        //播放动画UI集合
        private HashSet<IController> playAnimControlSet;
        //是否有正在播放动画的UI
        private bool IsPlayingAnimInSet 
        {
            get { return playAnimControlSet.Count != 0; }
        }

        public override void Initialize()
        {
            //初始化
            registeredControllerDic = new Dictionary<string, IWindowController>();
            windowQue = new Queue<WindowQueueData>();
            windowHistory = new Stack<WindowQueueData>();
            playAnimControlSet = new HashSet<IController>();

            popLayer = transform.parent.Find("WindowPopLayer").GetComponent<WindowPopLayer>();
        }

        protected override void Register(string uiName, IWindowController controller)
        {
            base.Register(uiName, controller);
            controller.ShowAnimationOver += OnShowAnimationFinished;
            controller.HideAnimationOver += OnHideAnimationFinished;
            controller.CloseUI += OnCloseByWindow;
        }
        protected override void Unregister(string uiName, IWindowController controller)
        {
            base.Unregister(uiName, controller);
            controller.ShowAnimationOver -= OnShowAnimationFinished;
            controller.HideAnimationOver -= OnHideAnimationFinished;
            controller.CloseUI -= OnCloseByWindow;
        }

        public override void ShowUI(IWindowController uiController)
        {
            ShowUI<IWindowProperties>(uiController, null);
        }

        public override void ShowUI<TProperties>(IWindowController uiController, TProperties properties)
        {
            IWindowProperties windowProp = properties as IWindowProperties;

            
            if (ShouldEnqueue(uiController, windowProp))
            {
                //如果要进入队列
                EnqueueWindow(uiController, properties as IWindowProperties);
            }
            else
            {
                //不进入队列
                DoShow(uiController, windowProp);
            }
        }

        public override void HideUI(IWindowController uiController)
        {
            if (uiController == CurrentWindow)
            {
                windowHistory.Pop();
                AddAnimSet(uiController);
                uiController.Hide();

                CurrentWindow = null;

                if (windowQue.Count > 0)
                {
                    ShowNextInQueue();
                }
                else if (windowHistory.Count > 0)
                {
                    ShowPreviousInHistory();
                }
            }
            else
            {
                Debug.LogError(string.Format("[UIFramework]: 想关闭的[{0}]界面不是当前正在显示的[{1}]界面",
                        uiController.UIName, CurrentWindow != null ? CurrentWindow.UIName : "null"));
            }
        }

        public override void HideAll(bool playAnimation = true)
        {
            base.HideAll(playAnimation);
            CurrentWindow = null;
            popLayer.RefreshBK();
            windowHistory.Clear();
        }

        public override void SetUIParent(IController controller, Transform uiTransform)
        {
            
            IWindowController window = controller as IWindowController;

            if (window == null)
            {
                Debug.LogError("[UIFramework]: [" + uiTransform.name + "]界面不是窗口");
            }
            else
            {
                //如果要以弹出方式打开，将层级设置到弹窗层级
                if (window.IsPopup)
                {
                    popLayer.AddToLayer(uiTransform);
                    return;
                }
            }

            base.SetUIParent(controller, uiTransform);
        }

        //进入窗口队列
        private void EnqueueWindow(IWindowController controller, IWindowProperties properties)
        {
            windowQue.Enqueue(new WindowQueueData(controller,properties));
        }
        //判断是否应该进入队列
        private bool ShouldEnqueue(IWindowController controller, IWindowProperties windowProp)
        {
            //当前没有其他要显示的窗口时
            if (CurrentWindow == null && windowQue.Count == 0)
            {
                return false;
            }

            //窗口属性的队列优先级为强制显示时
            if (windowProp != null && windowProp.OverridePrefabProperties)
                return windowProp.WindowQueuePriority != E_WindowPriority.ForceAhead;
            else
                return controller.WindowQueuePriority != E_WindowPriority.ForceAhead;
        }
        //显示之前的窗口
        private void ShowPreviousInHistory()
        {
            if (windowHistory.Count > 0)
            {
                WindowQueueData windowData = windowHistory.Pop();
                DoShow(windowData);
            }
        }
        //显示下一个窗口
        private void ShowNextInQueue()
        {
            if (windowQue.Count > 0)
            {
                WindowQueueData windowData = windowQue.Dequeue();
                DoShow(windowData);
            }
        }
        //显示窗口具体处理
        private void DoShow(IWindowController controller, IWindowProperties properties)
        {
            DoShow(new WindowQueueData(controller, properties));
        }
        //显示窗口具体处理
        private void DoShow(WindowQueueData windowData)
        {
            if (CurrentWindow == windowData.controller)
            {
                //如果显示的是当前窗口
                Debug.LogWarning(string.Format("[UIFramework]: 尝试重复打开当前窗口[{0}]", CurrentWindow.UIName));
                return;
            }
            else if (CurrentWindow != null
                     && CurrentWindow.HideWhenOtherForeground
                     && !windowData.controller.IsPopup)
            {
                //如果要隐藏当前窗口
                CurrentWindow.Hide();
            }

            //显示
            windowHistory.Push(windowData);
            AddAnimSet(windowData.controller);

            if (windowData.controller.IsPopup)
            {
                //如果以弹出方式显示,显示背景
                popLayer.ShowBK();
            }

            windowData.Show();
            
            CurrentWindow = windowData.controller;
        }
        //显示动画结束时
        private void OnShowAnimationFinished(IController controller)
        {
            RemoveAnimSet(controller);
        }
        //隐藏动画结束时
        private void OnHideAnimationFinished(IController controller)
        {
            RemoveAnimSet(controller);
            IWindowController window = controller as IWindowController;
            if (window.IsPopup)
            {
                popLayer.RefreshBK();
            }
        }
        //关闭窗口
        private void OnCloseByWindow(IController controller)
        {
            HideUI(controller as IWindowController);
        }
        //加入正在播放动画的UI集合
        private void AddAnimSet(IController controller)
        {
            playAnimControlSet.Add(controller);
            animStartPlay?.Invoke();
        }
        //移除正在播放动画的UI集合
        private void RemoveAnimSet(IController controller)
        {
            playAnimControlSet.Remove(controller);
            if (!IsPlayingAnimInSet)
            {
                //如果所有动画播放完毕
                allAnimOverPlay?.Invoke();
            }
        }
    }
}