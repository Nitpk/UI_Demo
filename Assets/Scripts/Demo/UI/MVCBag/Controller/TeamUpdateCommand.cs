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
    /// 角色阵容更新命令
    /// </summary>
    public class TeamUpdateCommand : BaseCommand
    {
        public override void Execute(INotification notification)
        {
            var characterModel = (UIFacade.Instance.RetrieveProxy(ProxyNames.CharacterModel)) as CharacterModel;
            var cInfo = characterModel.characterDisplay;

            //上阵角色已经在阵容中，结束
            if (cInfo.isOnTeam) return;

            characterModel.characterDic[cInfo.id].isOnTeam = true;

            //让同类型的下阵
            if (characterModel.teamList[cInfo.type - 1] != null)
            {
                int cId = characterModel.teamList[cInfo.type - 1].id;
                characterModel.characterDic[cId].isOnTeam = false;
            }
            characterModel.teamList[cInfo.type - 1] = cInfo;

            //更新阵容框显示
            var teamMediator = (UIFacade.Instance
                .RetrieveMediator(MediatorNames.TeamMediator.ToString()))
                as TeamMediator;

            teamMediator.SetTeam(cInfo);

            //更新背包的标记
            var bagMediator = (UIFacade.Instance
                .RetrieveMediator(MediatorNames.CharacterBagMediator.ToString()))
                as CharacterBagMediator;

            bagMediator.UpdateBagUI(characterModel.GetList(characterModel.characterType),false);

            //更新上阵按钮
            var displayMediator = (UIFacade.Instance.RetrieveMediator(MediatorNames.CharacterDisplayMediator.ToString()))
                as CharacterDisplayMediator;

            displayMediator.SetTeamButtonState(true);

            //高亮
            SendNotification(Notifications.HIGHLIGHT,cInfo.id);

        }
    }
}