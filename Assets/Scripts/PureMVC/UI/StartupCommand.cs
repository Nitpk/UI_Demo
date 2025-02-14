/*
 * 作者：阳贻凡
 */
using Demo;
using PureMVC.Interfaces;
using UnityEngine;

namespace MVC_UIFramework
{
    /// <summary>
    /// 初始化
    /// 启动命令
    /// </summary>
    public class StartupCommand : BaseCommand
    {
        public override void Execute(INotification notification)
        {
            var uiSettings = notification.Body as UISettings;

            // 初始化UI层级
            UIFacade.Instance.InitUILayerMgr(uiSettings.uiLayerMgr);

            // 注册所有UI
            foreach (var entry in uiSettings.entries)
            {
                var instance = Object.Instantiate(entry.prefab);
                instance.SetActive(!uiSettings.inactiveUIOnInit);
                UIFacade.Instance.UILayerMgrInstance.AddToLayer(instance, entry.layerType);

                //Debug.Log(entry.uiKey);
                var mediator = CreateMediator(entry.uiKey, instance, entry.layerType);

                //Debug.Log(mediator.MediatorName);
                UIFacade.Instance.RegisterMediator(mediator);

            }

            //注册所有Proxy
            //TODO 目前是手动写，后面有时间可以改成读取配置然后注册
            UIFacade.Instance.RegisterProxy(new CharacterModel());
        }

        private BaseMediator CreateMediator(MediatorNames uiKey, GameObject instance, E_UILayerType layer)
        {
            return uiKey switch
            {
                MediatorNames.CharacterMenuMediator => new CharacterMenuMediator(instance, layer),
                MediatorNames.BagMediator => new BagMediator(instance, layer),
                MediatorNames.CharacterBagMediator => new CharacterBagMediator(instance, layer),
                MediatorNames.CharacterDisplayMediator => new CharacterDisplayMediator(instance, layer),
                MediatorNames.TeamMediator => new TeamMediator(instance, layer),
                _ => null
            };
        }
    }
}