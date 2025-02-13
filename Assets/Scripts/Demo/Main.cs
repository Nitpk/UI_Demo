/*
 * 作者：阳贻凡
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MVC_UIFramework;

namespace Demo 
{
    //Demo主入口
    public class Main : MonoBehaviour
    {
        [Header("UI设置模板")]
        [SerializeField]
        private UISettings uiSettings;


        private void Awake()
        {
            UIFacade.Instance.Startup(uiSettings);
        }

        private void Start()
        {
            UIFacade.Instance.SendNotification(Notifications.MENU_SHOW);
        }
    }
}


