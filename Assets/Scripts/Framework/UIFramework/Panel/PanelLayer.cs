/*
 * 作者：阳贻凡
 */
using UnityEngine;
using System.Collections.Generic;

namespace UIFramework
{
    /// <summary>
    /// 面板层
    /// </summary>
    public class PanelLayer : UILayer<IPanelController>
    {
        [SerializeField]
        [Tooltip("内部层级")]
        private PanelLayerPriorityList priorityLayers = null;

        public override void Initialize()
        {
            //初始化
            registeredControllerDic = new Dictionary<string, IPanelController> ();

            var dataList = new List<PanelLayerPriorityListData>
            {
                new PanelLayerPriorityListData(E_PanelLayerPriority.Default, transform),
                new PanelLayerPriorityListData(E_PanelLayerPriority.Foreground,transform.parent.Find("PanelForeLayer"))
            };
            priorityLayers = new PanelLayerPriorityList(dataList);
        }

        public override void SetUIParent(IController controller, Transform uiTransform)
        {
            IPanelController ctrl = controller as IPanelController;
            if (ctrl != null)
            {
                SetLayer(ctrl.PanelLayerPriority, uiTransform);
            }
            else
            {
                base.SetUIParent(controller, uiTransform);
            }
        }

        public override void ShowUI(IPanelController controller)
        {
            controller.Show();
        }

        public override void ShowUI<TProps>(IPanelController controller, TProps properties)
        {
            controller.Show(properties);
        }

        public override void HideUI(IPanelController controller)
        {
            controller.Hide();
        }
        /// <summary>
        /// 判断面板是否在显示中
        /// </summary>
        /// <param name="uiName">UI名</param>
        public bool IsPanelVisible(string uiName)
        {
            IPanelController controller;
            if (registeredControllerDic.TryGetValue(uiName, out controller))
            {
                return controller.IsVisible;
            }

            return false;
        }

        //设置内部层级
        private void SetLayer(E_PanelLayerPriority priority, Transform uiTransform)
        {
            Transform trans;
            if (priorityLayers.LayerDic.TryGetValue(priority, out trans))
            {
                trans = transform;
                uiTransform.SetParent(trans, false);
            }
            else
            {
                Debug.LogError(string.Format("[UIFramework]: 面板内部层级没有设置{0}对应的GameObject",priority.ToString()));
            }
        }
    }
}