/*
 * 作者：阳贻凡
 */
namespace UIFramework
{
    /// <summary>
    /// 面板层内部层级
    /// </summary>
    public enum E_PanelLayerPriority
    {
        /// <summary>
        /// 默认层级
        /// </summary>
        Default = 0,
        /// <summary>
        /// 前景层级，比默认层级更后渲染，比窗口层级更前渲染
        /// </summary>
        Foreground = 1
    }
}