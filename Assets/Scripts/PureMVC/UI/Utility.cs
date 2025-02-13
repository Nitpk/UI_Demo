/*
 * 作者：阳贻凡
 */
using UnityEngine;

namespace MVC_UIFramework
{
    /// <summary>
    /// 工具类
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// 寻找组件，如果没有，自动添加
        /// </summary>
        /// <typeparam name="T">组件</typeparam>
        public static T AddComponentIfMissing<T>(this GameObject gObj) where T : Component
        {
            if (!gObj.TryGetComponent<T>(out var component))
                component = gObj.AddComponent<T>();
            return component;
        }
    }
}