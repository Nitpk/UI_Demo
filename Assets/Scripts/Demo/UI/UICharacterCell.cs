/*
 * 作者：阳贻凡
 */
using UnityEngine;
using UnityEngine.UI;
using TFrameWork;
using System.Text;
using MVC_UIFramework;

namespace Demo
{
    /// <summary>
    /// UI角色框
    /// </summary>
    public class UICharacterCell : MonoBehaviour
    {
        //临时 角色图片路径
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

        //选中高亮
        [SerializeField] private Image highlight;

        //当前角色信息
        private int cId;

        //是否初始化
        private bool isInited = false;

        //字符串常量
        private const string LevelStr = "Lv.";
        private const string TypeStr = "类型";

        private static StringBuilder stringBuilder = new StringBuilder(20);

        private void Awake()
        {
            Init();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            if (isInited == true) return;

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
                group.Find("StarImage (4)").GetComponent<Image>()
            };

            characterImage = transform.Find("CharacterImage").GetComponent<Image>();
            teamImage = transform.Find("TeamImage").GetComponent<Image>();
            highlight = GetComponent<Image>();

            cId = -1;

            isInited = true;
        }

        private void Start()
        {
            //监听事件
            characterBtn.onClick.AddListener(OnClick);
        }
        private void OnDestroy()
        {
            //取消监听
            characterBtn.onClick.RemoveListener(OnClick);
        }
        
        /// <summary>
        /// 点击
        /// </summary>
        public void OnClick()
        {
            UIFacade.Instance.SendNotification(Notifications.HIGHLIGHT, cId);
            SetHighlight(true);
        }

        /// <summary>
        /// 更新显示信息
        /// </summary>
        public void UpdateUIDisplay(CharacterInfo cInfo)
        {
            //未初始化时
            if (isInited == false) return;

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
            SetTeam(cInfo.isOnTeam);

            //设置角色是否高亮
            var highlightID = ((UIFacade.Instance.RetrieveProxy(ProxyNames.CharacterModel))as CharacterModel).highlightID;
            SetHighlight(highlightID == cInfo.id);

            cId = cInfo.id;
        }
        /// <summary>
        /// 设置阵容
        /// </summary>
        public void SetTeam(bool isOnTeam)
        {

            if (isOnTeam) teamImage.canvasRenderer.SetAlpha(1f);
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

