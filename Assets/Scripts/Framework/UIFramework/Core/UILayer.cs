/*
 * 作者：阳贻凡
 */
using System.Collections.Generic;
using UnityEngine;

namespace UIFramework
{
    /// <summary>
    /// UI层级基类，负责管理层级中的所有UI界面
    /// </summary>
    public abstract class UILayer <TUIController>: MonoBehaviour where TUIController : IController
    {
        /// <summary>
        /// 已注册的UI字典
        /// </summary>
        protected Dictionary<string, TUIController> registeredControllerDic;
        /// <summary>
        /// 显示UI界面
        /// </summary>
        /// <param name="uiController">UI界面</param>
        public abstract void ShowUI(TUIController uiController);
        /// <summary>
        /// 显示UI界面（带UI属性参数）
        /// </summary>
        /// <typeparam name="TProperties">属性类型</typeparam>
        /// <param name="uiController">UI界面</param>
        /// <param name="properties">UI属性</param>
        public abstract void ShowUI<TProperties>(TUIController uiController, TProperties properties) where TProperties : IProperties;
        /// <summary>
        /// 隐藏UI界面
        /// </summary>
        /// <param name="uiController">UI界面</param>
        public abstract void HideUI(TUIController uiController);
        /// <summary>
        /// 初始化layer层级
        /// </summary>
        public virtual void Initialize()
        {
            registeredControllerDic = new Dictionary<string, TUIController>();
        }
        /// <summary>
        /// 将UI界面设为当前层的子物体
        /// </summary>
        /// <param name="uiController">UI界面</param>
        /// <param name="uiTransform">界面的位置组件</param>
        public virtual void SetUIParent(IController uiController,Transform uiTransform)
        {
            uiTransform.SetParent(this.transform,false);
        }
        /// <summary>
        /// 隐藏层级中所有界面
        /// </summary>
        /// <param name="PlayAnimation">隐藏时是否播放动画</param>
        public virtual void HideAll(bool playAnimation = true)
        {
            foreach(TUIController controller in registeredControllerDic.Values)
            {
                controller.Hide(playAnimation);
            }
        }
        /// <summary>
        /// 注册UI界面具体处理
        /// </summary>
        /// <param name="uiName">UI名</param>
        /// <param name="uiController">UI界面</param>
        protected virtual void Register(string uiName,TUIController uiController)
        {
            uiController.UIName = uiName;
            registeredControllerDic.Add(uiName, uiController);
            uiController.UIDestroyed += OnUIDestroyed;
        }
        /// <summary>
        /// 注销UI界面具体处理
        /// </summary>
        /// <param name="uiName">UI名</param>
        /// <param name="uiController">UI界面</param>
        protected virtual void Unregister(string uiName,TUIController uiController)
        {
            uiController.UIDestroyed -= OnUIDestroyed;
            registeredControllerDic.Remove(uiName);
        }
        /// <summary>
        /// 注册UI界面
        /// </summary>
        /// <param name="uiName">UI名字</param>
        /// <param name="uiController">UI界面</param>
        public void RegisterUI(string uiName,TUIController uiController)
        {
            if (registeredControllerDic.ContainsKey(uiName))
            {
                //如果已经注册该界面
                Debug.LogError(string.Format("[UIFramework]:[{0}]界面已经被注册到层级中",uiName));
            }
            else
            {
                //如果还没注册
                Register(uiName,uiController);
            }
        }
        /// <summary>
        /// 注销UI界面
        /// </summary>
        /// <param name="uiName">UI名字</param>
        /// <param name="uiController">UI界面</param>
        public void UnregisterUI(string uiName, TUIController uiController)
        {
            if (registeredControllerDic.ContainsKey(uiName))
            {
                //如果已经注册该界面
                Unregister(uiName,uiController);
            }
            else
            {
                //如果还没注册
                Debug.LogError(string.Format("[UIFramework]:[{0}]界面没有注册到层级中，无法注销", uiName));
            }
        }
        /// <summary>
        /// 通过UI名字显示UI
        /// </summary>
        /// <param name="uiName">UI名</param>
        public void ShowUIByName(string uiName)
        {
            TUIController uiController;
            if (registeredControllerDic.TryGetValue(uiName,out uiController))
            {
                ShowUI(uiController);
            }
            else
            {
                Debug.LogError(string.Format("[UIFramework]:[{0}]界面没有注册到层级中", uiName));
            }
        }
        /// <summary>
        /// 通过UI名字显示UI(带属性参数)
        /// </summary>
        /// <typeparam name="TProperties">属性类型</typeparam>
        /// <param name="uiName">UI名</param>
        /// <param name="properitise">属性</param>
        public void ShowUIByName<TProperties>(string uiName,TProperties properitise)where TProperties : IProperties
        {
            TUIController uiController;
            if (registeredControllerDic.TryGetValue(uiName, out uiController))
            {
                ShowUI<TProperties>(uiController,properitise);
            }
            else
            {
                Debug.LogError(string.Format("[UIFramework]:[{0}]界面没有注册到层级中", uiName));
            }
        }
        /// <summary>
        /// 通过UI名字隐藏UI
        /// </summary>
        /// <param name="uiName">UI名</param>
        public void HideUIByName(string uiName)
        {
            TUIController uiController;
            if (registeredControllerDic.TryGetValue(uiName, out uiController))
            {
                HideUI(uiController);
            }
            else
            {
                Debug.LogError(string.Format("[UIFramework]:[{0}]界面没有注册到层级中", uiName));
            }
        }
        /// <summary>
        /// 根据UI名看是否注册到层级中
        /// </summary>
        /// <param name="uiName">UI名</param>
        public bool IsRegistered(string uiName)
        {
            return registeredControllerDic.ContainsKey(uiName);
        }
        //当界面销毁时
        private void OnUIDestroyed(IController uiController)
        {
            if (!string.IsNullOrEmpty(uiController.UIName)
                && registeredControllerDic.ContainsKey(uiController.UIName))
            {
                UnregisterUI(uiController.UIName, (TUIController)uiController);
            }
        }
    }
}