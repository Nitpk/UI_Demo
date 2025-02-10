/*
 * 作者：阳贻凡
 */
using System;
using UnityEngine;

namespace UIFramework
{
    /// <summary>
    /// UI动画组件基类
    /// </summary>
    public abstract class UIAnim : MonoBehaviour
    {
        /// <summary>
        /// 播放动画
        /// </summary>
        /// <param name="target">播放动画的目标</param>
        /// <param name="onFinished">动画结束时回调</param>
        public abstract void Play(Transform target,Action onFinished);
    }
}