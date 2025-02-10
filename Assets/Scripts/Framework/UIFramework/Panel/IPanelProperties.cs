/*
 * 作者：阳贻凡
 */
namespace UIFramework
{
    /// <summary>
    /// 面板属性接口
    /// </summary>
    public interface IPanelProperties : IProperties
    {
        /// <summary>
        /// 面板层内部层级优先级
        /// </summary>
        E_PanelLayerPriority PanelLayerPriority { get; set; }
    }
}