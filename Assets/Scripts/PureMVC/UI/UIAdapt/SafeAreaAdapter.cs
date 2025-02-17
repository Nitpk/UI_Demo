/*
 * 作者：阳贻凡
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MVC_UIFramework
{
    /// <summary>
    /// 安全区适配
    /// 使用时注意只能挂载在界面的GameObject上，因为每一个UI层级我都作为一个单独的画布并挂了缩放组件
    /// </summary>
    public class SafeAreaAdapter : UIAdapter
    {

        private RectTransform rect;
        //随便获取一个层级的画布缩放组件都行
        private static CanvasScaler scaler;

        /// <summary>
        /// 初始化缩放组件
        /// </summary>
        /// <param name="scaler"></param>
        public static void Init(CanvasScaler scaler)
        {
            SafeAreaAdapter.scaler = scaler;
            //Debug.Log(scaler.name);
        }

        private void Awake()
        {
            if(scaler == null)
                Init(GetComponentInParent<AdaptCanvasScaler>());
            rect = GetComponent<RectTransform>();
            Adapt();
        }

        private void Update()
        {
            if (adaptEveryFrame)
            {
                Adapt();
            }
        }

        public override void Adapt()
        {
            if (scaler == null) return;

            //得到安全区范围并设置
            var safeArea = Screen.safeArea;

            int width = (int)(scaler.referenceResolution.x * (1 - scaler.matchWidthOrHeight) +
                scaler.referenceResolution.y * Screen.width / Screen.height * scaler.matchWidthOrHeight);
            int height = (int)(scaler.referenceResolution.y * scaler.matchWidthOrHeight -
              scaler.referenceResolution.x * Screen.height / Screen.width * (scaler.matchWidthOrHeight - 1));

            float ratio = scaler.referenceResolution.y * scaler.matchWidthOrHeight / Screen.height -
                scaler.referenceResolution.x * (scaler.matchWidthOrHeight - 1) / Screen.width;

            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(safeArea.position.x * ratio, safeArea.position.y * ratio);
            rect.offsetMax = new Vector2(safeArea.position.x * ratio + safeArea.width * ratio - width, -(height - safeArea.position.y * ratio - safeArea.height * ratio));
        }

    }
}