/*
 * 作者：阳贻凡
 */
using System.Collections;
using UnityEngine.UI;
using TFrameWork;
using UIFramework;

namespace Demo
{
    /// <summary>
    /// 角色菜单界面
    /// </summary>
    public class CharacterMenuPanel : PanelController
    {
        #region UI组件
        //关闭按钮
        private Button backBtn;
        //侠客
        private Toggle characterTog;
        //图鉴
        private Toggle bookTog;
        #endregion
        protected override void InitUI()
        {
            //获取UI组件
            backBtn = transform.Find("BackBtn").GetComponent<Button>();

            var toggleGroupTrans = transform.Find("ToggleGroup");
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
        protected override void OnShow()
        {
            characterTog.isOn = true;
            CharacterTogValueChange(true);
        }

        //点击侠客选项
        private void CharacterTogValueChange(bool isOn)
        {
            //触发事件
            EventCenter.Instance.TriggerEvent(E_EventType.E_ClickCharacter,isOn);
        }


    }
}