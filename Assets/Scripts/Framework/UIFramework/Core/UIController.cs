/*
 * 作者：阳贻凡
 */
using System;
using System.Collections;
using UnityEngine;

namespace UIFramework
{
    /// <summary>
    /// UI界面基类
    /// </summary>
    /// <typeparam name="TProperties">属性类型</typeparam>
    public abstract class UIController<TProperties> : MonoBehaviour, IController where TProperties : IProperties
    {
        #region 检视面板参数
        [Header("动画")]
        [Tooltip("界面显示时的动画")]
        [SerializeField]
        private UIAnim inAnim;
        /// <summary>
        /// 界面显示时的动画
        /// </summary>
        public UIAnim InAnim
        {
            get { return inAnim; }
            set { inAnim = value; }
        }

        [Tooltip("界面隐藏时的动画")]
        [SerializeField]
        private UIAnim outAnim;
        /// <summary>
        /// 界面隐藏时的动画
        /// </summary>
        public UIAnim OutAnim
        {
            get { return outAnim; }
            set { outAnim = value; }
        }

        [Header("界面属性")]
        [Tooltip("界面属性")]
        [SerializeField]
        private TProperties properties;
        /// <summary>
        /// 界面属性
        /// </summary>
        protected TProperties Properties 
        { 
            get { return properties; } 
            set { properties = value; }
        }
        #endregion

        #region 实现Controller接口
        public string UIName { get; set; }

        public bool IsVisible { get; private set; }

        public Action<IController> ShowAnimationOver { get; set; }
        public Action<IController> HideAnimationOver {  get; set; }
        public Action<IController> CloseUI {  get; set; }
        public Action<IController> UIDestroyed {  get; set; }

        public void Hide(bool playAnimation = true)
        {
            PlayAnim(playAnimation? outAnim:null,OnHideAnimOver,false);
            OnHide();
        }

        public void Show(IProperties properties = null)
        {
            //设置属性
            if(properties != null)
            {
                if(properties is TProperties)
                {
                    SetProperties((TProperties)properties);
                }
                else
                {
                    Debug.LogError(string.Format("[UIFramework]:[{0}]界面的属性类型不正确，应为{1}，实际为{2}",UIName,typeof(TProperties),properties.GetType()));
                    return;
                }
            }

            OnSetProperties();
            OnShow();

            //播放动画
            if (!gameObject.activeSelf)
            {
                PlayAnim(inAnim,OnShowAnimOver,true);
            }
            else
            {
                //如果已经处于显示状态了，则不播放动画，只执行回调事件
                ShowAnimationOver?.Invoke(this);
            }
        }
        #endregion

        protected virtual void Awake()
        {
            InitUI();
            AddListeners();
        }

        protected virtual void OnDestroy()
        {
            //销毁时执行
            UIDestroyed?.Invoke(this);

            //将事件清空，避免内存泄漏
            ShowAnimationOver = null;
            HideAnimationOver = null;
            CloseUI = null;
            UIDestroyed = null;
            RemoveListeners();
        }
        /// <summary>
        /// 初始化UI组件（实现时无需调用，Awake会调用）
        /// </summary>
        protected abstract void InitUI();
        /// <summary>
        /// 添加事件监听（实现时无需调用，Awake会调用）
        /// </summary>
        protected abstract void AddListeners();
        /// <summary>
        /// 移除事件监听（实现时无需调用，Destroy会调用）
        /// </summary>
        protected abstract void RemoveListeners();
        /// <summary>
        /// 设置属性
        /// </summary>
        protected virtual void SetProperties(TProperties properties)
        {
            this.properties = properties;
        }
        /// <summary>
        /// 在设置好属性后触发
        /// </summary>
        protected virtual void OnSetProperties() { }
        /// <summary>
        /// 在界面隐藏时触发
        /// </summary>
        protected virtual void OnHide() { }
        /// <summary>
        /// 在界面显示时触发
        /// </summary>
        protected virtual void OnShow() { }
        //播放动画
        private void PlayAnim(UIAnim anim,Action onFinished,bool isVisible)
        {
            if(anim == null)
            {
                //没有动画时只设置显隐
                gameObject.SetActive(isVisible);
                onFinished?.Invoke();
            }
            else
            {
                if (isVisible && !gameObject.activeSelf)
                {
                    gameObject.SetActive(true);
                }

                anim.Play(this.transform,onFinished);

            }

        }
        //显示动画结束
        private void OnShowAnimOver()
        {
            IsVisible = true;
            ShowAnimationOver?.Invoke(this);
        }
        //隐藏动画结束
        private void OnHideAnimOver()
        {
            IsVisible = false;
            gameObject.SetActive(false);

            HideAnimationOver?.Invoke(this);
        }
    }
}