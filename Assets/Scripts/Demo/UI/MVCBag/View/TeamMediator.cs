/*
 * 作者：阳贻凡
 */
using MVC_UIFramework;
using PureMVC.Interfaces;
using System.Collections.Generic;
using UnityEngine;
namespace Demo
{
    /// <summary>
    /// 阵容中介
    /// </summary>
    public class TeamMediator : BaseMediator
    {
        #region UI组件
        //阵容格子
        [SerializeField]
        private List<UITeamCell> cells;
        #endregion
        public TeamMediator(GameObject viewGO, E_UILayerType e_UILayerType)
        : base(MediatorNames.TeamMediator.ToString(), viewGO, e_UILayerType) { }

        protected override void Init()
        {
            //初始化
            cells = new List<UITeamCell>(5)
            {
                rootObject.transform.Find("TeamCell1").GetComponent<UITeamCell>(),
                rootObject.transform.Find("TeamCell2").GetComponent<UITeamCell>(),
                rootObject.transform.Find("TeamCell3").GetComponent<UITeamCell>(),
                rootObject.transform.Find("TeamCell4").GetComponent<UITeamCell>(),
                rootObject.transform.Find("TeamCell5").GetComponent<UITeamCell>()
            };
        }
        public override string[] ListNotificationInterests()
        {
            return new string[]
            {
                Notifications.CHARACTER_HIDE,
            };
        }

        public override void HandleNotification(INotification notification)
        {
            switch (notification.Name)
            {
                case Notifications.CHARACTER_HIDE:
                    Hide();
                    break;
            }
        }

        protected override void AddListeners(){}
        protected override void RemoveListeners(){}

        /// <summary>
        /// 设置上阵UI
        /// </summary>
        /// <param name="cInfo"></param>
        public void SetTeam(CharacterInfo cInfo)
        {
            //根据角色类型，选择对应的阵容格子
            cells[cInfo.type - 1].UpdateUIDisplay(cInfo);
        }

        /// <summary>
        /// 设置高亮
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isHighLight"></param>
        public void HighLight(E_CharacterType type,bool isHighLight,int cid)
        {
            if(cells[(int)type - 1].currentCID == cid)
                cells[(int)type - 1].SetHighlight(isHighLight);
        }
    }
}