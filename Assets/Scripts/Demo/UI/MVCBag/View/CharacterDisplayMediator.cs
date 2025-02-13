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
    /// 角色展示中介
    /// </summary>
    public class CharacterDisplayMediator : BaseMediator
    {
        #region UI组件
        //角色模型展示位置
        private RectTransform characterPos;
        //角色信息
        private Text qualityText;
        private Text typeText;
        private Text levelText;
        private Text nameText;
        private Image[] starGroup;
        //上阵按钮
        private Button teamBtn;
        private Text teamText;
        #endregion

        //字符串常量
        private const string LevelStr = "Lv.";
        private const string TypeStr = "类型";
        private const string OnTeamStr = "已上阵";
        private const string TeamStr = "上阵";

        //TODO 临时，后续改到资源加载里
        private GameObject characterLoaded;


        public CharacterDisplayMediator(GameObject viewGO, E_UILayerType e_UILayerType) 
            : base(MediatorNames.CharacterDisplayMediator.ToString(),viewGO,e_UILayerType) { }

        protected override void Init()
        {
            //初始化
            characterPos = rootObject.GetComponent<RectTransform>();

            qualityText = rootObject.transform.Find("QualityText").GetComponent<Text>();
            typeText = rootObject.transform.Find("TypeText").GetComponent<Text>();
            levelText = rootObject.transform.Find("LevelText").GetComponent<Text>();
            nameText = rootObject.transform.Find("NameText").GetComponent<Text>();

            var group = rootObject.transform.Find("StarGroup");
            starGroup = new Image[5] {
                group.Find("StarImage (4)").GetComponent<Image>(),
                group.Find("StarImage (3)").GetComponent<Image>(),
                group.Find("StarImage (2)").GetComponent<Image>(),
                group.Find("StarImage (1)").GetComponent<Image>(),
                group.Find("StarImage").GetComponent<Image>(),
            };

            teamBtn = rootObject.transform.Find("TeamBtn").GetComponent<Button>();
            teamText = teamBtn.transform.Find("Text (Legacy)").GetComponent<Text>();
        }
        protected override void AddListeners()
        {
            teamBtn.onClick.AddListener(OnClick);
        }

        protected override void RemoveListeners()
        {
            teamBtn.onClick.RemoveListener(OnClick);
        }
        public override string[] ListNotificationInterests()
        {
            return new string[]
            {
                Notifications.CHARACTER_HIDE,
                Notifications.HIGHLIGHT
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

        /// <summary>
        /// 更新角色UI
        /// </summary>
        /// <param name="cInfo"></param>
        public void UpdateCharacter(CharacterInfo cInfo)
        {
            //显示角色模型
            //TODO 根据角色信息加载对应角色模型

            //将UI本地坐标转换为世界坐标
            Vector3 pos = characterPos.TransformPoint(Vector3.zero);
            //设置角色模型位置
            //TODO 临时先使用Res同步
            GameObject  character = Resources.Load<GameObject>("Character");
            if (characterLoaded == null)
                characterLoaded = Object.Instantiate(character, rootObject.transform);
            characterLoaded.transform.position = new Vector3(pos.x, pos.y, characterLoaded.transform.position.z);

            //更新角色名字
            nameText.text = cInfo.name;
            //更新角色等级
            strBuilder.Clear();
            strBuilder.Append(LevelStr);
            strBuilder.Append(cInfo.level);
            levelText.text = strBuilder.ToString();
            //更新角色类型
            strBuilder.Clear();
            strBuilder.Append(TypeStr);
            strBuilder.Append(cInfo.type);
            typeText.text = strBuilder.ToString();

            //更新角色品质和星级
            SetStar((E_CharacterQuality)cInfo.quality);

            //更新角色是否上阵
            teamText.text = cInfo.isOnTeam ? OnTeamStr : TeamStr;
            
        }
        //点击上阵按钮
        private void OnClick()
        {
            SendNotification(Notifications.CHARACTER_TEAM_UPDATE);
        }
        /// <summary>
        /// 更新上阵按钮
        /// </summary>
        public void SetTeamButtonState(bool isOnTeam)
        {
            //更新上阵UI
            if(isOnTeam)
                teamText.text = OnTeamStr;
            else
                teamText.text = TeamStr;
        }

        // 设置品质和星级
        private void SetStar(E_CharacterQuality quality)
        {
            //设置品质
            qualityText.text = CharacterQuality.qualityStr[(int)quality - 1];

            //设置星级
            for (int i = 0; i < starGroup.Length; i++)
            {
                if (i < (int)quality) starGroup[i].canvasRenderer.SetAlpha(1f);
                else starGroup[i].canvasRenderer.SetAlpha(0f);
            }
        }

    }
}