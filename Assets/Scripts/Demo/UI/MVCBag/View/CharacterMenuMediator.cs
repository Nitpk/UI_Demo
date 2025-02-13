/*
 * 作者：阳贻凡
 */
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using MVC_UIFramework;
using PureMVC.Interfaces;

namespace Demo
{
    /// <summary>
    /// 角色背包菜单栏中介
    /// </summary>
    public class CharacterMenuMediator : BaseMediator
    {
        #region UI组件
        //关闭按钮
        private Button backBtn;
        //侠客
        private Toggle characterTog;
        //图鉴
        private Toggle bookTog;
        #endregion
        public CharacterMenuMediator(GameObject viewGO,E_UILayerType e_UILayerType) 
            : base(MediatorNames.CharacterMenuMediator.ToString(),viewGO,e_UILayerType) { }

        //初始化
        protected override void Init()
        {
            //获取UI组件
            backBtn = rootObject.transform.Find("BackBtn").GetComponent<Button>();

            var toggleGroupTrans = rootObject.transform.Find("ToggleGroup");
            characterTog = toggleGroupTrans.Find("CharacterToggle").GetComponent<Toggle>();
            bookTog = toggleGroupTrans.Find("BookToggle").GetComponent<Toggle>();
        }

        protected override void AddListeners()
        {
            characterTog.onValueChanged.AddListener(CharacterTogValueChange);
        }

        protected override void RemoveListeners()
        {
            characterTog.onValueChanged.RemoveListener(CharacterTogValueChange);
        }

        public override string[] ListNotificationInterests()
        {
            return new string[]
            {
                Notifications.MENU_SHOW
            };
        }

        public override void HandleNotification(INotification notification)
        {
            switch (notification.Name)
            {
                case Notifications.MENU_SHOW:
                    Show();
                    break;
            }
        }

        protected override void OnShow()
        {
            //显示时，默认显示侠客界面
            characterTog.isOn = true;
            CharacterTogValueChange(true);
        }
        //点击侠客选项
        private void CharacterTogValueChange(bool isOn)
        {
            //触发事件
            if(isOn)
                UIFacade.Instance.SendNotification(Notifications.CHARACTER_SHOW);
            else
                UIFacade.Instance.SendNotification(Notifications.CHARACTER_HIDE);
        }

    }
}