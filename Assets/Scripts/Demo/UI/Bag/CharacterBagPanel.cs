/*
 * 作者：阳贻凡
 */
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TFrameWork;
using UIFramework;
using System.Collections.Generic;
using System;

namespace Demo
{
    /// <summary>
    /// 角色背包面板
    /// </summary>
    public class CharacterBagPanel : PanelController
    {
        #region UI组件
        //增加背包容量
        [SerializeField]
        private Button addBagNumBtn;
        //背包容量
        [SerializeField]
        private Text bagNumText;
        //角色列表
        [SerializeField]
        private CharacterViewList characterViewList;
        //角色类型选项
        [SerializeField]
        private List<Toggle> typeToggles;
        #endregion
        //角色背包
        private CharacterBag characterBag;
        protected override void InitUI()
        {
            //初始化
            addBagNumBtn = transform.Find("AddBtn").GetComponent<Button>();
            bagNumText = transform.Find("NumText").GetComponent<Text>();
            characterViewList = transform.Find("Scroll View").GetComponent<CharacterViewList>();

            typeToggles = new List<Toggle>(6);
            var togGroup = transform.Find("ToggleGroup");
            typeToggles.Add(togGroup.Find("AllToggle").GetComponent<Toggle>());
            typeToggles.Add(togGroup.Find("Type1Toggle").GetComponent<Toggle>());
            typeToggles.Add(togGroup.Find("Type2Toggle").GetComponent<Toggle>());
            typeToggles.Add(togGroup.Find("Type3Toggle").GetComponent<Toggle>());
            typeToggles.Add(togGroup.Find("Type4Toggle").GetComponent<Toggle>());
            typeToggles.Add(togGroup.Find("Type5Toggle").GetComponent<Toggle>());

            characterBag = new CharacterBag(100);

        }
        protected override void AddListeners()
        {
            addBagNumBtn.onClick.AddListener(OnAddBagNum);

            foreach (var typeTog in typeToggles)
            {
                typeTog.onValueChanged.AddListener(OnClickTog);
            }

            EventCenter.Instance.AddEventListener<bool>(E_EventType.E_ClickCharacter, OnClickCharacter);
            EventCenter.Instance.AddEventListener(E_EventType.E_CharacterBagNumChange, UpdateBagNum);
        }
        protected override void RemoveListeners()
        {
            addBagNumBtn.onClick.RemoveListener(OnAddBagNum);

            foreach (var typeTog in typeToggles)
            {
                typeTog.onValueChanged.RemoveListener(OnClickTog);
            }

            EventCenter.Instance.RemoveEventListener<bool>(E_EventType.E_ClickCharacter, OnClickCharacter);
            EventCenter.Instance.RemoveEventListener(E_EventType.E_CharacterBagNumChange, UpdateBagNum);
        }

        protected override void OnShow()
        {
            UpdateBagNum();
            typeToggles[0].isOn = true;
            UpdateBagUI(E_CharacterType.All);
            UpdateTeam();
        }

       
        // 更新背包容量信息
        private void UpdateBagNum()
        {
            bagNumText.text = characterBag.CurrentNum + "/" + characterBag.MaxNum;
        }
        //更新背包列表
        private void UpdateBagUI(E_CharacterType type)
        {
            //根据背包信息初始化显示
            characterViewList.Initlize(characterBag.GetList(type), true);
        }
        //更新阵容
        private void UpdateTeam(CharacterInfo cInfo = null)
        {
            if (cInfo == null)
            {
                //找到所有上阵角色并更新
                List<CharacterInfo> characters = characterBag.GetList(0);
                int cnt = 0;
                foreach (CharacterInfo c in characters)
                {
                    if (c.isOnTeam)
                    {
                        EventCenter.Instance.TriggerEvent<(int, bool)>(E_EventType.E_Team,(c.id,true));
                        cnt++;
                    }
                    if (cnt == 5) break;
                }
            }
            else
            {
                //更新对应角色的阵容
                EventCenter.Instance.TriggerEvent<(int, bool)>(E_EventType.E_Team, (cInfo.id, true));
            }
        }

        //当点击侠客事件触发时
        private void OnClickCharacter(bool isOn)
        {
            if (isOn) Show();
            else Hide();
        }
        //增加背包容量
        private void OnAddBagNum()
        {
            //TODO 增加背包容量

            EventCenter.Instance.TriggerEvent(E_EventType.E_CharacterBagNumChange);
        }
        //点击角色类型
        private void OnClickTog(bool isOn)
        {
            for (int i=0;i<typeToggles.Count;i++)
            {
                if (typeToggles[i].isOn)
                {
                    //更新背包内容
                    if (!Enum.IsDefined(typeof(E_CharacterType), i))
                        Debug.LogError("背包内容更新时，角色类型无效");

                    UpdateBagUI((E_CharacterType)i);
                    return;
                }
            }
        }
    }
}