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
    /// </summary>
    public class SafeAreaAdapter : UIAdapter
    {
        private RectTransform rect;
        private static CanvasScaler scaler;

        public static void Init(CanvasScaler scaler)
        {
            SafeAreaAdapter.scaler = scaler;
        }

        private void Awake()
        {
            Init(GameObject.FindObjectOfType<CanvasScaler>());
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