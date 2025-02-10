/*
 * 作者：阳贻凡
 */
namespace UIFramework
{
    /// <summary>
    /// 窗口队列优先级
    /// </summary>
    public enum E_WindowPriority
    {
        /// <summary>
        /// 打开时，强制显示在当前窗口前面
        /// </summary>
        ForceAhead = 0,
        /// <summary>
        /// 如果已经有打开的窗口，先进入队列等待前面的窗口关闭时在打开
        /// </summary>
        Enqueue = 1
    }

}
