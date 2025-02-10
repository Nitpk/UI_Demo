/*
 * 作者：阳贻凡
 */
using System;

namespace UIFramework 
{
    /// <summary>
    /// UI界面对应须实现的基础接口
    /// </summary>
    public interface IController
    {
        /// <summary>
        /// UI界面名字，注意不能同名，作为唯一标识
        /// </summary>
        string UIName { get; set; }
        /// <summary>
        /// 界面是否显示
        /// </summary>
        bool IsVisible { get; }
        /// <summary>
        /// 显示动画结束回调
        /// </summary>
        Action<IController> ShowAnimationOver {  get; set; }
        /// <summary>
        /// 隐藏动画结束回调
        /// </summary>
        Action<IController> HideAnimationOver {  get; set; }
        /// <summary>
        /// 关闭UI界面回调
        /// </summary>
        Action<IController> CloseUI {  get; set; }
        /// <summary>
        /// UI销毁时回调
        /// </summary>
        Action<IController> UIDestroyed {  get; set; }
        /// <summary>
        /// 显示界面
        /// </summary>
        /// <param name="properties">属性</param>
        void Show(IProperties properties = null);
        /// <summary>
        /// 隐藏界面
        /// </summary>
        /// <param name="playAnimation">隐藏时是否播放动画</param>
        void Hide(bool playAnimation = true);

    }
}


