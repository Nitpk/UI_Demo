/*
 * 作者：阳贻凡
 */
using Demo;
using PureMVC.Patterns.Facade;
using UnityEngine;

namespace MVC_UIFramework
{
    /// <summary>
    /// UI门面
    /// </summary>
    public class UIFacade : Facade
    {
        public static UIFacade Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UIFacade();
                }
                return instance as UIFacade;
            }
        }

        /// <summary>
        /// UI层级管理
        /// </summary>
        public UILayerMgr UILayerMgrInstance { get; private set; }


        //初始化controller
        protected override void InitializeController()
        {
            base.InitializeController();

            //绑定通知和命令
            RegisterCommand(Notifications.START_UP, () => new StartupCommand());
            RegisterCommand(Notifications.CHARACTER_SHOW, () => new ShowBagCommand());
            RegisterCommand(Notifications.CHARACTER_TYPE_UPDATE, () => new UpdateCharacterTypeCommand());
            RegisterCommand(Notifications.CHARACTER_TEAM_UPDATE, () => new TeamUpdateCommand());
            RegisterCommand(Notifications.HIGHLIGHT,() => new HighlightCommand());

        }

        /// <summary>
        /// 启动UI框架
        /// </summary>
        public void Startup(UISettings uiSettings)
        {
            SendNotification(Notifications.START_UP,uiSettings);
        }
        /// <summary>
        /// 初始化UI层级管理器
        /// </summary>
        public void InitUILayerMgr(GameObject uiLayerGObj)
        {
            if (UILayerMgrInstance != null) return;

            //实例化预制体并得到组件
            UILayerMgrInstance = Object.Instantiate(uiLayerGObj).GetComponent<UILayerMgr>();
            //初始化
            UILayerMgrInstance.Init();
        }

    }
}