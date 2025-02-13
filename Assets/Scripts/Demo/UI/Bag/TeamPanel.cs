/*
 * 作者：阳贻凡
 */
using System.Collections;
using UnityEngine;
using TFrameWork;
using UIFramework;
using System.Collections.Generic;

namespace Demo
{
    /// <summary>
    /// 阵容面板
    /// </summary>
    public class TeamPanel : PanelController
    {
        #region UI组件
        //阵容格子
        [SerializeField]
        private List<UITeamCell> cells;
        #endregion

        protected override void InitUI()
        {
            //初始化
            cells = new List<UITeamCell>(5)
            {
                transform.Find("TeamCell1").GetComponent<UITeamCell>(),
                transform.Find("TeamCell2").GetComponent<UITeamCell>(),
                transform.Find("TeamCell3").GetComponent<UITeamCell>(),
                transform.Find("TeamCell4").GetComponent<UITeamCell>(),
                transform.Find("TeamCell5").GetComponent<UITeamCell>()
            };
        }
        protected override void AddListeners()
        {
            EventCenter.Instance.AddEventListener<bool>(E_EventType.E_ClickCharacter, OnClickCharacter);
            EventCenter.Instance.AddEventListener<(int, bool)>(E_EventType.E_Team, Team);
        }
        protected override void RemoveListeners()
        {
            EventCenter.Instance.RemoveEventListener<bool>(E_EventType.E_ClickCharacter, OnClickCharacter);
            EventCenter.Instance.RemoveEventListener<(int, bool)>(E_EventType.E_Team, Team);
        }

        //当点击侠客事件触发时
        private void OnClickCharacter(bool isOn)
        {
            if (isOn) Show();
            else Hide();
        }
        
        //阵容更新
        private void Team((int characterID, bool isOnTeam) arg)
        {
            if (!arg.isOnTeam) return;

            //更新角色是否上阵

            Dictionary<int, CharacterInfo> cDic = BinaryMgr.Instance.GetTable<CharacterInfoContainer>().dataDic;

            
            CharacterInfo cInfo = cDic[arg.characterID];
            cInfo.isOnTeam = true;

            //让同类型的下阵
            if (cells[cInfo.type - 1] != null)
            {
                int cId = cells[cInfo.type - 1].currentCID;
                EventCenter.Instance.TriggerEvent<(int, bool)>(E_EventType.E_Team, (cId, false));
                cDic[cId].isOnTeam = false;
            }

            //上阵
            SetTeam(cInfo);
        }
        // 设置上阵
        private void SetTeam(CharacterInfo cInfo)
        {
            //根据角色类型，选择对应的阵容格子
            cells[cInfo.type - 1].UpdateUIDisplay(cInfo);
        }
    }
}