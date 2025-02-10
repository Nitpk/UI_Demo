/*
 * 作者：阳贻凡
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIFramework
{
    /// <summary>
    /// 专门为弹出窗口设的层级，属于窗口层级的一部分
    /// </summary>
    public class WindowPopLayer : MonoBehaviour
    {
        [Tooltip("弹出窗口背景")]
        [SerializeField]
        private GameObject backgroundObj = null;

        //弹窗列表
        private List<GameObject> uiList = new List<GameObject>();

        private void Awake()
        {
            //初始化
            backgroundObj = transform.Find("Background").gameObject;
        }


        /// <summary>
        /// 加入此层级
        /// </summary>
        /// <param name="uiTransform">UI位置组件</param>
        public void AddToLayer(Transform uiTransform)
        {
            uiTransform.SetParent(this.transform,false);
            uiList.Add(uiTransform.gameObject);
        }
        /// <summary>
        /// 显示背景
        /// </summary>
        public void ShowBK()
        {
            if (backgroundObj != null)
            {
                backgroundObj.SetActive(true);
                backgroundObj.transform.SetAsLastSibling();
            }
            else
            {
                Debug.LogError("[UIFramework]:弹窗层级的背景引用为空");
            }
        }
        /// <summary>
        /// 刷新背景
        /// </summary>
        public void RefreshBK()
        {
            for (int i = 0; i < uiList.Count; i++)
            {
                if (uiList[i] != null
                    && uiList[i].activeSelf)
                {
                    backgroundObj.SetActive(true);
                    return;
                }
            }

            backgroundObj.SetActive(false);
        }
    }
}