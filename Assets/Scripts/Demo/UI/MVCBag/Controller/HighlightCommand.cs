/*
 * 作者：阳贻凡
 */
using MVC_UIFramework;
using PureMVC.Interfaces;

namespace Demo
{
    /// <summary>
    /// 高亮角色命令
    /// </summary>
    public class HighlightCommand : BaseCommand
    {
        public override void Execute(INotification notification)
        {
            int cID = (int)notification.Body;

            var characterModel = (UIFacade.Instance.RetrieveProxy(ProxyNames.CharacterModel)) as CharacterModel;

            //阵容高亮更新显示
            var teamMediator = (UIFacade.Instance
                .RetrieveMediator(MediatorNames.TeamMediator.ToString()))
                as TeamMediator;

            teamMediator.HighLight((E_CharacterType)characterModel.characterDic[characterModel.highlightID].type
                , false, characterModel.highlightID);
            teamMediator.HighLight((E_CharacterType)characterModel.characterDic[cID].type,true,cID);

            //记录id
            characterModel.highlightID = cID;

            //角色背包高亮更新显示
            var bagMediator = (UIFacade.Instance
                .RetrieveMediator(MediatorNames.CharacterBagMediator.ToString()))
                as CharacterBagMediator;

            bagMediator.UpdateBagUI(characterModel.GetList(characterModel.characterType),false);

            //角色展示更新
            var displayMediator = (UIFacade.Instance
                .RetrieveMediator(MediatorNames.CharacterDisplayMediator.ToString()))
                as CharacterDisplayMediator;

            characterModel.characterDisplay = characterModel.characterDic[cID];

            displayMediator.UpdateCharacter(characterModel.characterDisplay);
        }

    }
}