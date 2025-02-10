/*
 * 作者：阳贻凡
 */
namespace UIFramework
{
    /// <summary>
    /// 面板基类
    /// </summary>
    public abstract class PanelController<TProperties> : UIController<TProperties>, IPanelController 
        where TProperties : IPanelProperties
    {
        /// <summary>
        /// 面板层内部层级优先级
        /// </summary>
        public E_PanelLayerPriority PanelLayerPriority
        {
            get
            {
                if (Properties != null)
                {
                    return Properties.PanelLayerPriority;
                }
                else
                {
                    return E_PanelLayerPriority.Default;
                }
            }
        }

        protected sealed override void SetProperties(TProperties props)
        {
            base.SetProperties(props);
        }
    }
    /// <summary>
    /// 默认面板
    /// </summary>
    public abstract class PanelController : PanelController<PanelProperties> { }
}