/*
 * 作者：阳贻凡
 */
using System;
using UnityEngine;

namespace UIFramework
{
    /// <summary>
    /// 窗口属性
    /// </summary>
    [Serializable]
    public class WindowProperties : IWindowProperties
    {
        #region 检视面板参数
        [Tooltip("当其他界面在前面时，是否隐藏")]
        [SerializeField]
        protected bool hideWhenOtherForeground = true;
        [Tooltip("窗口队列优先级")]
        [SerializeField]
        protected E_WindowPriority windowQueuePriority = E_WindowPriority.ForceAhead;
        [Tooltip("是否弹出界面")]
        [SerializeField]
        protected bool isPopup = false;
        #endregion
        public E_WindowPriority WindowQueuePriority 
        { 
            get { return windowQueuePriority; } 
            set { windowQueuePriority = value; } 
        }
        public bool HideWhenOtherForeground 
        { 
            get { return hideWhenOtherForeground; } 
            set { hideWhenOtherForeground = value; }
        }
        public bool IsPopup 
        { 
            get { return isPopup; } 
            set { isPopup = value; } 
        }
        public bool OverridePrefabProperties {get; set; }

        public WindowProperties() 
        {
            hideWhenOtherForeground = true;
            windowQueuePriority = E_WindowPriority.ForceAhead;
            isPopup = false;
        }
        public WindowProperties(bool overridePrefabProperties = false)
        {
            HideWhenOtherForeground = false;
            WindowQueuePriority = E_WindowPriority.ForceAhead;
            OverridePrefabProperties = overridePrefabProperties;
        }
        public WindowProperties(E_WindowPriority priority, bool hideWhenOtherForeground = false, bool overridePrefabProperties = false)
        {
            WindowQueuePriority = priority;
            HideWhenOtherForeground |= hideWhenOtherForeground;
            OverridePrefabProperties |= overridePrefabProperties;
        }

    }
}