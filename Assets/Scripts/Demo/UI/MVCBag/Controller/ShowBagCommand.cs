/*
 * 作者：阳贻凡
 */
using MVC_UIFramework;
using PureMVC.Interfaces;

namespace Demo
{
    /// <summary>
    /// 显示角色背包命令
    /// </summary>
    public class ShowBagCommand : BaseCommand
    {

        public override void Execute(INotification notification)
        {
            //显示背包
            var bagMediator = (UIFacade.Instance.RetrieveMediator(MediatorNames.CharacterBagMediator.ToString()))
                as CharacterBagMediator;

            bagMediator.Show();

            var cModel = (CharacterModel)(UIFacade.Instance.RetrieveProxy(ProxyNames.CharacterModel));

            bagMediator.UpdateBagNum(cModel);
            var cList = cModel.GetList(E_CharacterType.All);
            cModel.characterType = E_CharacterType.All;
            bagMediator.UpdateBagUI(cList);

            //找到所有上阵角色并更新
            var teamMediator = (UIFacade.Instance.RetrieveMediator(MediatorNames.TeamMediator.ToString()))
                as TeamMediator;
            for (int i = 0; i < 5; i++)
            {
                teamMediator.SetTeam(cModel.teamList[i]);
            }
            teamMediator.Show();

            //显示角色
            var displayMediator = (UIFacade.Instance.RetrieveMediator(MediatorNames.CharacterDisplayMediator.ToString()))
                as CharacterDisplayMediator;
            displayMediator.Show();

            SendNotification(Notifications.HIGHLIGHT, cList[0].id);
        }
    }
}