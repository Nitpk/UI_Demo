/*
 * 作者：阳贻凡
 */
using UnityEngine;
using UnityEngine.UI;

namespace MVC_UIFramework
{
    /// <summary>
    /// 缩放模式更改
    /// </summary>
    public class AdaptCanvasScaler : CanvasScaler
    {
        protected override void HandleScaleWithScreenSize()
        {
            //根据屏幕分辨率修改
            if (Screen.width / m_ReferenceResolution.x < Screen.height / m_ReferenceResolution.y)
                matchWidthOrHeight = 0f;
            else
                matchWidthOrHeight = 1f;
            base.HandleScaleWithScreenSize();
        }
    }
}