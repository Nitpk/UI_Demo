/*
 * 作者：阳贻凡
 */
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MVC_UIFramework
{
    /// <summary>
    /// 分辨率自适应
    /// </summary>
    [RequireComponent(typeof(CanvasScaler))]
    public class UIAdapter : MonoBehaviour
    {
        [Header("参考分辨率")]
        [SerializeField] 
        private Vector2 referenceResolution = new(1920, 1080);
        [Header("匹配模式")]
        [SerializeField] 
        private CanvasScaler.ScreenMatchMode matchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

        private void Awake()
        {
            var scaler = GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = referenceResolution;
            scaler.screenMatchMode = matchMode;
        }
    }
}