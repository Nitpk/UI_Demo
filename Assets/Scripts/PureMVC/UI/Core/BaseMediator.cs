/*
 * 作者：阳贻凡
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PureMVC.Interfaces;
using PureMVC.Patterns.Mediator;
using UnityEngine;
using UnityEngine.UI;

namespace MVC_UIFramework
{
    /// <summary>
    /// 表现层
    /// UI中介基类，负责协调View与其他部分的交互
    /// </summary>
    public abstract class BaseMediator : Mediator
    {
        /// <summary>
        /// UI根物体
        /// </summary>
        protected readonly GameObject rootObject;
        /// <summary>
        /// UI层级
        /// </summary>
        protected E_UILayerType layerType;

        protected static StringBuilder strBuilder = new(20);

        protected BaseMediator(string mediatorName, GameObject viewComponent, E_UILayerType layer)
            : base(mediatorName, viewComponent)
        {
            rootObject = viewComponent;
            layerType = layer;
            
            Init();
            AddListeners();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        protected abstract void Init();
        /// <summary>
        /// 监听事件
        /// </summary>
        protected abstract void AddListeners();
        /// <summary>
        /// 移除事件
        /// </summary>
        protected abstract void RemoveListeners();

        /// <summary>
        /// 显示
        /// </summary>
        public void Show()
        {
            if (rootObject.activeSelf) return;

            rootObject.SetActive(true);

            OnShow();
        }
        /// <summary>
        /// 显示
        /// </summary>
        protected virtual void OnShow() { }
        /// <summary>
        /// 隐藏
        /// </summary>
        public void Hide()
        {
            if (!rootObject.activeSelf) return;

            rootObject.SetActive(false);

            OnHide();
        }
        /// <summary>
        /// 隐藏
        /// </summary>
        protected virtual void OnHide(){ }
        /// <summary>
        /// 销毁
        /// </summary>
        public void Destroy()
        {
            RemoveListeners();
        }
    }
}