/*
 * 作者：阳贻凡
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;

namespace Demo 
{
    //Demo主入口
    public class Main : MonoBehaviour
    {
        [Header("UI设置模板")]
        [SerializeField]
        private UISettings uiSettings;

        private UIMgr uiMgr;

        private void Awake()
        {
            //初始化
            uiMgr = uiSettings.CreateUIInstance();
        }

        private void Start()
        {
            uiMgr.ShowPanel(UIName.CharacterMenuPanel);
        }
    }
}


