/*
 * 作者：阳贻凡
 */
namespace MVC_UIFramework
{
    /// <summary>
    /// UI层级枚举
    /// </summary>
    public enum E_UILayerType
    {
        /// <summary>
        /// 背景层
        /// </summary>
        Background = 0,
        /// <summary>
        /// 默认层
        /// </summary>
        Normal = 100,
        /// <summary>
        /// 弹出窗口层
        /// </summary>
        Popup = 200,
        /// <summary>
        /// 提示信息层
        /// </summary>
        Tips = 300,
        /// <summary>
        /// 加载界面层
        /// </summary>
        Loading = 400,
        /// <summary>
        /// 系统界面层
        /// </summary>
        System = 999
    }
}