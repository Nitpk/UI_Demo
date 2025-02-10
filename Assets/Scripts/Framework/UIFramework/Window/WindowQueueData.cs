/*
 * 作者：阳贻凡
 */
namespace UIFramework
{
    /// <summary>
    /// 窗口队列数据
    /// </summary>
    public struct WindowQueueData
    {
        /// <summary>
        /// UI窗口
        /// </summary>
        public readonly IWindowController controller;
        /// <summary>
        /// UI窗口属性
        /// </summary>
        public readonly IWindowProperties properties;

        public WindowQueueData(IWindowController controller, IWindowProperties properties)
        {
            this.controller = controller;
            this.properties = properties;
        }
        /// <summary>
        /// 封装show，方便窗口队列调用
        /// </summary>
        public void Show()
        {
            controller.Show(properties);
        }
    }
}