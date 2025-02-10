/*
 * 作者：阳贻凡
 */
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TFrameWork;
using UnityEngine;
using UnityEngine.UI;

namespace Demo
{
    /// <summary>
    /// 阵容角色框
    /// </summary>
    public class UITeamCell : MonoBehaviour
    {
        //头像图片路径
        private static string path = "/ArtRes/";
        //角色信息
        [SerializeField] private Text qualityText;
        [SerializeField] private Text levelText;
        [SerializeField] private Image[] starGroup;
        [SerializeField] private Image headImage;
        //头像按钮
        [SerializeField] private Button teamBtn;
        //选中高光
        [SerializeField] private Image highlight;
        //当前角色信息
        private CharacterInfo characterInfo;
        /// <summary>
        /// 当前角色信息
        /// </summary>
        public CharacterInfo CurrentCInfo => characterInfo;

        //字符串常量
        private const string LevelStr = "Lv.";

        private static StringBuilder stringBuilder = new StringBuilder(20);

        private void Awake()
        {
            //初始化
            teamBtn = GetComponent<Button>();

            qualityText = transform.Find("QualityText").GetComponent<Text>();
            levelText = transform.Find("LevelText").GetComponent<Text>();

            var group = transform.Find("StarGroup");
            starGroup = new Image[5] {
                group.Find("StarImage").GetComponent<Image>(),
                group.Find("StarImage (1)").GetComponent<Image>(),
                group.Find("StarImage (2)").GetComponent<Image>(),
                group.Find("StarImage (3)").GetComponent<Image>(),
                group.Find("StarImage (4)").GetComponent<Image>(),
            };

            headImage = transform.Find("HeadImage").GetComponent<Image>();
            highlight = GetComponent<Image>();
        }

        private void Start()
        {
            //监听事件
            teamBtn.onClick.AddListener(OnClick);

            EventCenter.Instance.AddEventListener<int>(E_EventType.E_Highlight, Highlight);
        }
        private void Highlight(int characterID)
        {
            if (characterInfo == null) return;

            SetHighlight(characterID==characterInfo.id);
        }
        
        private void OnDestroy()
        {
            //取消监听
            teamBtn.onClick.RemoveListener(OnClick);

            EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_Highlight, Highlight);   
        }
        /// <summary>
        /// 点击
        /// </summary>
        public void OnClick()
        {
            EventCenter.Instance.TriggerEvent<int>(E_EventType.E_Highlight,characterInfo.id);
        }

        /// <summary>
        /// 更新显示信息
        /// </summary>
        public void UpdateUIDisplay(CharacterInfo cInfo)
        {
            characterInfo = cInfo;
            //更新角色图片
            headImage.sprite = EditorResMgr.Instance.LoadEditorRes<Sprite>(path + cInfo.headPath);
            //更新角色品质和星级
            SetStar((E_CharacterQuality)cInfo.quality);
            //更新角色等级
            stringBuilder.Clear();
            stringBuilder.Append(LevelStr);
            stringBuilder.Append(cInfo.level);
            levelText.text = stringBuilder.ToString();
        }
        /// <summary>
        /// 设置高光
        /// </summary>
        /// <param name="isHighlight"></param>
        public void SetHighlight(bool isHighlight)
        {
            if(isHighlight)highlight.color=Color.yellow;
            else highlight.color=Color.white;
        }
        /// <summary>
        /// 设置品质和星级
        /// </summary>
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

