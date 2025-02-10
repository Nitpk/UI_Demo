/*
 * 作者：阳贻凡
 */
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TFrameWork;
using UIFramework;
using UnityEditor;
using System.Text;

namespace Demo
{
    /// <summary>
    /// 角色展示面板
    /// </summary>
    public class CharacterDisplayPanel : PanelController
    {
        #region UI组件
        //角色模型展示位置
        [SerializeField]
        private RectTransform characterPos;
        //角色信息
        [SerializeField] 
        private Text qualityText;
        [SerializeField] 
        private Text typeText;
        [SerializeField] 
        private Text levelText;
        [SerializeField] 
        private Text nameText;
        [SerializeField] 
        private Image[] starGroup;
        //上阵按钮
        [SerializeField]
        private Button teamBtn;
        [SerializeField]
        private Text teamText;
        #endregion

        //当前角色id
        private int cId;
        private bool isOnTeam;

        //模拟加载的对应角色模型
        public GameObject character;
        private GameObject characterLoaded;

        //字符串常量
        private const string LevelStr = "Lv.";
        private const string TypeStr = "类型";
        private const string OnTeamStr = "已上阵";
        private const string TeamStr = "上阵";

        private StringBuilder stringBuilder;

        protected override void InitUI()
        {
            //初始化
            characterPos = GetComponent<RectTransform>();

            qualityText = transform.Find("QualityText").GetComponent<Text>();
            typeText = transform.Find("TypeText").GetComponent<Text>();
            levelText = transform.Find("LevelText").GetComponent<Text>();
            nameText = transform.Find("NameText").GetComponent<Text>();

            var group = transform.Find("StarGroup");
            starGroup = new Image[5] {
                group.Find("StarImage (4)").GetComponent<Image>(),
                group.Find("StarImage (3)").GetComponent<Image>(),
                group.Find("StarImage (2)").GetComponent<Image>(),
                group.Find("StarImage (1)").GetComponent<Image>(),
                group.Find("StarImage").GetComponent<Image>(),
            };

            teamBtn = transform.Find("TeamBtn").GetComponent<Button>();
            teamText = teamBtn.transform.Find("Text (Legacy)").GetComponent<Text>();

            stringBuilder = new StringBuilder(20);
        }
        protected override void AddListeners()
        {
            teamBtn.onClick.AddListener(OnClick);
            EventCenter.Instance.AddEventListener<bool>(E_EventType.E_ClickCharacter, OnClickCharacter);
            EventCenter.Instance.AddEventListener<int>(E_EventType.E_Highlight, Highlight);
        }
        protected override void RemoveListeners()
        {
            teamBtn.onClick.RemoveListener(OnClick);
            EventCenter.Instance.RemoveEventListener<bool>(E_EventType.E_ClickCharacter, OnClickCharacter);
            EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_Highlight, Highlight);
        }
        //当点击侠客事件触发时
        private void OnClickCharacter(bool isOn)
        {
            if (isOn) Show();
            else Hide();
        }
        private void Highlight(int characterID)
        {
            //显示对应角色
            UpdateCharacter(BinaryMgr.Instance.GetTable<CharacterInfoContainer>().dataDic[characterID]);
        }

        // 更新角色UI
        private void UpdateCharacter(CharacterInfo cInfo)
        {
            cId = cInfo.id;
            isOnTeam = cInfo.isOnTeam;
            //显示角色模型
            //TODO 加载对应角色模型

            //将UI本地坐标转换为世界坐标
            Vector3 pos = characterPos.TransformPoint(Vector3.zero);
            //设置角色模型位置
            if (characterLoaded == null)
                characterLoaded = Instantiate(character, this.gameObject.transform);
            characterLoaded.transform.position = new Vector3(pos.x, pos.y, characterLoaded.transform.position.z);

            //更新角色名字
            nameText.text = cInfo.name;
            //更新角色等级
            stringBuilder.Clear();
            stringBuilder.Append(LevelStr);
            stringBuilder.Append(cInfo.level);
            levelText.text = stringBuilder.ToString();
            //更新角色类型
            stringBuilder.Clear();
            stringBuilder.Append(TypeStr);
            stringBuilder.Append(cInfo.type);
            typeText.text = stringBuilder.ToString();

            //更新角色品质和星级
            SetStar((E_CharacterQuality)cInfo.quality);

            //更新角色是否上阵
            teamText.text = cInfo.isOnTeam ? OnTeamStr : TeamStr;
        }
        //点击上阵按钮
        private void OnClick()
        {
            if (isOnTeam) return;

            //更新角色是否上阵
            teamText.text = OnTeamStr;

            EventCenter.Instance.TriggerEvent<(int, bool)>(E_EventType.E_Team, (cId, true));
            EventCenter.Instance.TriggerEvent<int>(E_EventType.E_Highlight, cId);
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