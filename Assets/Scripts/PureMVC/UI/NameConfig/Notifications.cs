/*
 * 作者：阳贻凡
 */
namespace MVC_UIFramework
{
    /// <summary>
    /// 所有通知
    /// </summary>
    public static class Notifications 
    {
        /// <summary>
        /// 启动通知--uiSettings
        /// </summary>
        public const string START_UP = "START_UP";
        /// <summary>
        /// 显示菜单--无参数
        /// </summary>
        public const string MENU_SHOW = "MENU_SHOW";
        /// <summary>
        /// 侠客界面显示通知--CharacterModel
        /// </summary>
        public const string CHARACTER_SHOW = "CHARACTER_SHOW";
        /// <summary>
        /// 侠客界面隐藏通知--无参数
        /// </summary>
        public const string CHARACTER_HIDE = "CHARACTER_HIDE";
        /// <summary>
        /// 角色背包容量改变--CharacterModel
        /// </summary>
        public const string CHARACTER_BAG_NUM_CHANGE = "CHARACTER_BAG_NUM_CHANGE";
        /// <summary>
        /// 角色背包更新--CharacterModel
        /// </summary>
        public const string CHARACTER_BAG_UPDATE = "CHARACTER_BAG_UPDATE";
        /// <summary>
        /// 角色背包角色类型更新--CharacterModel
        /// </summary>
        public const string CHARACTER_TYPE_UPDATE = "CHARACTER_TYPE_UPDATE";
        /// <summary>
        /// 角色阵容更新--无参数
        /// </summary>
        public const string CHARACTER_TEAM_UPDATE = "CHARACTER_TEAM_UPDATE";
        /// <summary>
        /// 选中角色高亮--CharacterInfo.id
        /// </summary>
        public const string HIGHLIGHT = "HIGHLIGHT";
        
    }
}