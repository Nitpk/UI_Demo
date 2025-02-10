/*
 * 作者：阳贻凡
 */
namespace UIFramework
{
    /// <summary>
    /// UI窗口基类
    /// </summary>
    /// <typeparam name="TProperties">窗口属性类型</typeparam>
    public abstract class WindowController<TProperties> : UIController<TProperties>,IWindowController
        where TProperties : IWindowProperties
    {
        public bool HideWhenOtherForeground 
        { 
            get { return Properties.HideWhenOtherForeground; } 
        }

        public bool IsPopup
        {
            get { return Properties.IsPopup; }
        }

        public E_WindowPriority WindowQueuePriority
        {
            get { return Properties.WindowQueuePriority; }
        }

        /// <summary>
        /// 关闭UI窗口
        /// </summary>
        public virtual void Close()
        {
            CloseUI?.Invoke(this);
        }

        /// <summary>
        /// 设置窗口属性
        /// </summary>
        /// <param name="properties">属性</param>
        protected sealed override void SetProperties(TProperties properties)
        {
            //如果要覆盖Prefab中的设置
            if (properties != null
                && properties.OverridePrefabProperties)
            {
                Properties = properties;
            }
        }
        /// <summary>
        /// 窗口显示时，默认到最前面
        /// </summary>
        protected override void OnShow()
        {
            this.transform.SetAsLastSibling();
        }
    }

    /// <summary>
    /// 默认UI窗口
    /// </summary>
    public abstract class WindowController : WindowController<WindowProperties> { }
}