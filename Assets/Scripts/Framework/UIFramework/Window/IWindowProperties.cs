/*
 * 作者：阳贻凡
 */
namespace UIFramework
{
    /// <summary>
    /// UI窗口属性接口
    /// </summary>
    public interface IWindowProperties:IProperties
    {
        /// <summary>
        /// 窗口队列优先级
        /// </summary>
        E_WindowPriority WindowQueuePriority { get; set; }
        /// <summary>
        /// 当其他界面在前面时，是否隐藏
        /// </summary>
        bool HideWhenOtherForeground { get; set; }
        /// <summary>
        /// 是否弹出界面，显示背景阻挡射线检测，适合界面上的小弹窗等
        /// </summary>
        bool IsPopup {  get; set; }
        /// <summary>
        /// 是否覆盖掉在Prefab中配置的属性
        /// </summary>
        bool OverridePrefabProperties { get; set; }

    }
}
