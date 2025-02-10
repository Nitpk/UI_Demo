/*
 * 作者：阳贻凡
 */
namespace UIFramework
{
    /// <summary>
    /// 面板界面接口
    /// </summary>
    public interface IPanelController : IController
    {
        /// <summary>
        /// 面板层级优先级
        /// </summary>
        E_PanelLayerPriority PanelLayerPriority { get; }
    }
}