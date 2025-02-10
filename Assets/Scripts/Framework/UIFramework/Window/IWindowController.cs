/*
 * 作者：阳贻凡
 */
namespace UIFramework
{
    /// <summary>
    /// UI窗口控制器接口
    /// </summary>
    public interface IWindowController : IController
    {
        /// <summary>
        /// 窗口队列优先级
        /// </summary>
        E_WindowPriority WindowQueuePriority { get; }
        /// <summary>
        /// 当其他界面在前面时，是否隐藏
        /// </summary>
        bool HideWhenOtherForeground { get; }
        /// <summary>
        /// 是否以弹出的方式显示窗口
        /// </summary>
        bool IsPopup { get; }
    }
}