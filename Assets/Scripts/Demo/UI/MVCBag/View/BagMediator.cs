/*
 * 作者：阳贻凡
 */
using MVC_UIFramework;
using PureMVC.Interfaces;
using PureMVC.Patterns.Mediator;
using System.Collections;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace Demo
{
    /// <summary>
    /// 背包中介
    /// 负责管理所有的子中介
    /// </summary>
    public class BagMediator : BaseMediator
    {

        public BagMediator(GameObject viewGO, E_UILayerType e_UILayerType)
            : base(MediatorNames.BagMediator.ToString(), viewGO, e_UILayerType) { }

        protected override void Init()
        {
            //初始化子中介
            var menuGO = rootObject.transform.Find("CharacterMenuPanel").gameObject;
            var characterBagGO = rootObject.transform.Find("CharacterBagPanel").gameObject;
            var displayGO = rootObject.transform.Find("CharacterDisplayPanel").gameObject;
            var teamGO = rootObject.transform.Find("TeamPanel").gameObject;

            UIFacade.Instance.RegisterMediator(new CharacterMenuMediator(menuGO, layerType));
            UIFacade.Instance.RegisterMediator(new CharacterBagMediator(characterBagGO, layerType));
            UIFacade.Instance.RegisterMediator(new CharacterDisplayMediator(displayGO, layerType));
            UIFacade.Instance.RegisterMediator(new TeamMediator(teamGO, layerType));
        }
        protected override void AddListeners(){}

        protected override void RemoveListeners(){}

        public override string[] ListNotificationInterests()
        {
            return new string[]
            {
                Notifications.MENU_SHOW
            };

        }

        public override void HandleNotification(INotification notification)
        {
            switch(notification.Name)
            {
                case Notifications.MENU_SHOW:
                    Show();
                    break;
            }
        }
    }
}