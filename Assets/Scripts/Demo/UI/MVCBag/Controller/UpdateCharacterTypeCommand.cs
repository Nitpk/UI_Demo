/*
 * 作者：阳贻凡
 */
using System.Collections;
using UnityEngine;
using MVC_UIFramework;
using PureMVC.Interfaces;

namespace Demo
{
    /// <summary>
    /// 刷新背包的角色类型命令
    /// </summary>
    public class UpdateCharacterTypeCommand : BaseCommand
    {

        public override void Execute(INotification notification)
        {
            var cModel=(CharacterModel)(UIFacade.Instance.RetrieveProxy(ProxyNames.CharacterModel));

            cModel.characterType = (E_CharacterType)(notification.Body);
            SendNotification(Notifications.CHARACTER_BAG_UPDATE,cModel.GetList(cModel.characterType));
            
        }
    }
}