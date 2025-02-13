/*
 * 作者：阳贻凡
 */
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using MVC_UIFramework;
using PureMVC.Interfaces;
using System.Collections.Generic;

namespace Demo
{
    /// <summary>
    /// 角色背包中介
    /// </summary>
    public class CharacterBagMediator : BaseMediator
    {
        #region UI组件
        //增加背包容量
        private Button addBagNumBtn;
        //背包容量
        private Text bagNumText;
        //角色列表
        private CharacterViewList characterViewList;
        //角色类型选项
        private List<Toggle> typeToggles;
        #endregion

        public CharacterBagMediator(GameObject viewGO, E_UILayerType e_UILayerType)
            : base(MediatorNames.CharacterBagMediator.ToString(), viewGO, e_UILayerType) { }

        protected override void Init()
        {
            //初始化
            addBagNumBtn = rootObject.transform.Find("AddBtn").GetComponent<Button>();
            bagNumText = rootObject.transform.Find("NumText").GetComponent<Text>();
            characterViewList = rootObject.transform.Find("Scroll View").GetComponent<CharacterViewList>();

            typeToggles = new List<Toggle>(6);
            var togGroup = rootObject.transform.Find("ToggleGroup");
            typeToggles.Add(togGroup.Find("AllToggle").GetComponent<Toggle>());
            typeToggles.Add(togGroup.Find("Type1Toggle").GetComponent<Toggle>());
            typeToggles.Add(togGroup.Find("Type2Toggle").GetComponent<Toggle>());
            typeToggles.Add(togGroup.Find("Type3Toggle").GetComponent<Toggle>());
            typeToggles.Add(togGroup.Find("Type4Toggle").GetComponent<Toggle>());
            typeToggles.Add(togGroup.Find("Type5Toggle").GetComponent<Toggle>());
        }
        protected override void AddListeners()
        {
            addBagNumBtn.onClick.AddListener(OnAddBagNum);

            foreach (var typeTog in typeToggles)
            {
                typeTog.onValueChanged.AddListener(OnClickTog);
            }

        }
        protected override void RemoveListeners()
        {
            addBagNumBtn.onClick.RemoveListener(OnAddBagNum);

            foreach (var typeTog in typeToggles)
            {
                typeTog.onValueChanged.RemoveListener(OnClickTog);
            }
        }
        public override string[] ListNotificationInterests()
        {
            return new string[]
            {
                Notifications.CHARACTER_HIDE,
                Notifications.CHARACTER_BAG_NUM_CHANGE,
                Notifications.CHARACTER_BAG_UPDATE,
            };
        }

        public override void HandleNotification(INotification notification)
        {
            switch (notification.Name)
            {
                case Notifications.CHARACTER_HIDE:
                    Hide();
                    break;
                case Notifications.CHARACTER_BAG_NUM_CHANGE:
                    UpdateBagNum((CharacterModel)(notification.Body));
                    break;
                case Notifications.CHARACTER_BAG_UPDATE:
                    UpdateBagUI((List<CharacterInfo>)(notification.Body));
                    break;
            }
        }
        
        //背包容量UI更新
        private void OnAddBagNum()
        {
            SendNotification(Notifications.CHARACTER_BAG_NUM_CHANGE);
        }
        
        protected override void OnShow()
        {
            typeToggles[0].isOn = true;
        }

        /// <summary>
        /// 更新背包容量信息
        /// </summary>
        /// <param name="characterModel"></param>
        public void UpdateBagNum(CharacterModel characterModel)
        {
            if (characterModel == null) return;

            strBuilder.Clear();
            strBuilder.Append(characterModel.CurrentNum);
            strBuilder.Append("/");
            strBuilder.Append(characterModel.MaxNum);
            bagNumText.text = strBuilder.ToString();
        }
        /// <summary>
        /// 更新背包列表
        /// </summary>
        /// <param name="characterInfos"></param>
        public void UpdateBagUI(List<CharacterInfo> characterInfos,bool refresh = true)
        {
            //根据背包信息初始化显示
            characterViewList.Initlize(characterInfos, refresh);
        }

        //更新角色类型
        private void OnClickTog(bool isOn)
        {
            for (int i = 0; i < typeToggles.Count; i++)
            {
                if (typeToggles[i].isOn)
                {
                    SendNotification(Notifications.CHARACTER_TYPE_UPDATE, (E_CharacterType)i);
                    return;
                }
            }
        }

    }
}