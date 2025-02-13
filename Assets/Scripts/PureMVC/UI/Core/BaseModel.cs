/*
 * 作者：阳贻凡
 */
using PureMVC.Patterns.Proxy;
using Unity.Collections;

namespace MVC_UIFramework
{
    /// <summary>
    /// 模型层
    /// 数据模型基类
    /// </summary>
    public abstract class BaseModel : Proxy
    {
        public BaseModel(string proxyName) : base(proxyName)
        {
            SetData();
        }
         
        /// <summary>
        /// 关联数据和代理
        /// </summary>
        protected abstract void SetData();
    }
}