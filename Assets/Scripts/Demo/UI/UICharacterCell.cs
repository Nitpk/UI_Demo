/*
 * 作者：阳贻凡
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TFrameWork;
using System.Text;

namespace Demo
{
    /// <summary>
    /// UI角色框
    /// </summary>
    public class UICharacterCell : MonoBehaviour
    {
        //角色图片路径
        private static string path = "/ArtRes/";

        //角色信息
        [SerializeField] private Text qualityText;
        [SerializeField] private Text typeText;
        [SerializeField] private Text levelText;
        [SerializeField] private Image[] starGroup;
        [SerializeField] private Image characterImage;

        [SerializeField] private Button characterBtn;

        //是否上阵
        [SerializeField] private Image teamImage;
        public bool IsOnTeam { get; private set; }

        //选中高亮
        [SerializeField] private Image highlight;

        //当前角色信息
        private CharacterInfo characterInfo;
        private int cId;

        //字符串常量
        private const string LevelStr = "Lv.";
        private const string TypeStr = "类型";

        private static StringBuilder stringBuilder = new StringBuilder(20);

        private void Awake()
        {
            //初始化
            characterBtn = GetComponent<Button>();

            qualityText = transform.Find("QualityText").GetComponent<Text>();
            typeText = transform.Find("TypeText").GetComponent<Text>();
            levelText = transform.Find("LevelText").GetComponent<Text>();

            var group = transform.Find("StarGroup");
            starGroup = new Image[5] {
                group.Find("StarImage").GetComponent<Image>(),
                group.Find("StarImage (1)").GetComponent<Image>(),
                group.Find("StarImage (2)").GetComponent<Image>(),
                group.Find("StarImage (3)").GetComponent<Image>(),
                group.Find("StarImage (4)").GetComponent<Image>(),
            };

            characterImage = transform.Find("CharacterImage").GetComponent<Image>();
            teamImage = transform.Find("TeamImage").GetComponent<Image>();
            highlight = GetComponent<Image>();

        }

        private void Start()
        {
            //监听事件
            characterBtn.onClick.AddListener(OnClick);

            EventCenter.Instance.AddEventListener<int>(E_EventType.E_Highlight, Highlight);
            EventCenter.Instance.AddEventListener<(int,bool)>(E_EventType.E_Team, Team);
        }
        private void OnDestroy()
        {
            //取消监听
            characterBtn.onClick.RemoveListener(OnClick);

            EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_Highlight, Highlight);
            EventCenter.Instance.RemoveEventListener<(int, bool)>(E_EventType.E_Team, Team);
        }
        private void Highlight(int characterID)
        {
            cId = characterID;
            if (characterInfo == null) return;
           
            SetHighlight(characterID == characterInfo.id);
        }
        private void Team((int characterID, bool isOnTeam) arg)
        {
            //更新角色是否上阵
            if(characterInfo.id==arg.characterID)
                SetTeam(arg.isOnTeam);
        }
        
        /// <summary>
        /// 点击
        /// </summary>
        public void OnClick()
        {
            EventCenter.Instance.TriggerEvent(E_EventType.E_Highlight, characterInfo.id);
        }

        /// <summary>
        /// 更新显示信息
        /// </summary>
        public void UpdateUIDisplay(CharacterInfo cInfo)
        {
            characterInfo = cInfo;
            //更新角色图片
            characterImage.sprite = EditorResMgr.Instance.LoadEditorRes<Sprite>(path+cInfo.imagePath);
            //更新角色品质和星级
            SetStar((E_CharacterQuality)cInfo.quality);
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
            //更新角色是否上阵
            SetTeam(BinaryMgr.Instance.GetTable<CharacterInfoContainer>().dataDic[cInfo.id].isOnTeam);
            //设置角色是否高亮
            SetHighlight(cId == characterInfo.id);
        }
        /// <summary>
        /// 设置阵容
        /// </summary>
        public void SetTeam(bool isOnTeam)
        {
            IsOnTeam = isOnTeam;

            if (IsOnTeam) teamImage.canvasRenderer.SetAlpha(1f);
            else teamImage.canvasRenderer.SetAlpha(0f);
        }

        /// <summary>
        /// 设置高亮
        /// </summary>
        /// <param name="isHighlight">是否高亮</param>
        public void SetHighlight(bool isHighlight)
        {
            if (isHighlight) highlight.color = Color.yellow;
            else highlight.color = Color.white;
        }
        /// <summary>
        /// 设置品质和星级
        /// </summary>
        private void SetStar(E_CharacterQuality quality)
        {
            //设置品质
            qualityText.text = CharacterQuality.qualityStr[(int)quality-1];

            //设置星级
            for (int i=0;i<starGroup.Length;i++)
            {   
                if (i < (int)quality) starGroup[i].canvasRenderer.SetAlpha(1f);
                else starGroup[i].canvasRenderer.SetAlpha(0f);
            }
        }
    }
}

