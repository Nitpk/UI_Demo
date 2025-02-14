/*
 * 作者：阳贻凡
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MVC_UIFramework
{
    /// <summary>
    /// 水平或竖直自动适配
    /// </summary>
    public class HorizontalOrVerticalAdapter : UIAdapter
    {
        //方向类型
        public enum DrictionType
        {
            Horizontal,
            Vertical
        }
        [Header("UI间隙")]
        public float gap = 0;
        [Header("适配方向")]
        public DrictionType drictionType = DrictionType.Horizontal;
        private List<float> targetPos = new List<float>();
        private RectTransform selfRect;
        private RectTransform SelfRect
        {
            get
            {
                if (selfRect == null)
                {
                    selfRect = GetComponent<RectTransform>();
                }
                return selfRect;
            }
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
            //总长度
            float sum = 0;
            //组件个数
            int activityCount = 0;

            if (drictionType == DrictionType.Horizontal)
            {

                for (int i = 0; i < SelfRect.childCount; i++)
                {
                    if (i >= targetPos.Count) targetPos.Add(0);

                    if (SelfRect.GetChild(i).gameObject.activeInHierarchy)
                    {
                        activityCount++;
                        var childRect = SelfRect.GetChild(i).GetComponent<RectTransform>();
                        sum += childRect.rect.width;

                        if (activityCount > 1) sum += gap;

                        targetPos[i] = sum - childRect.rect.width;
                        childRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, targetPos[i], childRect.rect.width);
                    }
                }
                SelfRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sum);

            }else if (drictionType == DrictionType.Vertical)
            {
                for (int i = 0; i < SelfRect.childCount; i++)
                {
                    if (i >= targetPos.Count) targetPos.Add(0);

                    if (SelfRect.GetChild(i).gameObject.activeInHierarchy)
                    {
                        activityCount++;
                        var childRect = SelfRect.GetChild(i).GetComponent<RectTransform>();
                        sum += childRect.rect.height;

                        if (activityCount > 1) sum += gap;

                        targetPos[i] = sum - childRect.rect.height;
                        childRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, targetPos[i], childRect.rect.height);
                    }
                }
                SelfRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sum);
            }
        }
    }
}