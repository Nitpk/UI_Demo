/*
 * 作者：阳贻凡
 */
using System.Collections;
using UnityEngine;

namespace MVC_UIFramework
{
    /// <summary>
    /// 分辨率自适应基类
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public abstract class UIAdapter : MonoBehaviour
    {
        [Header("是否每帧适配")]
        public bool adaptEveryFrame = true;
        public abstract void Adapt();
        
    }
}