/*
 * 作者：阳贻凡
 */
namespace TFrameWork
{
    /// <summary>
    /// 事件类型
    /// </summary>
    public enum E_EventType
    {
        /// <summary>
        /// 点击侠客按钮事件--(bool isOn)
        /// </summary>
        E_ClickCharacter,
        /// <summary>
        /// 角色背包容量改变--无参数
        /// </summary>
        E_CharacterBagNumChange,
        /// <summary>
        /// 高亮事件--(int 角色id)
        /// </summary>
        E_Highlight,
        /// <summary>
        /// 阵容更新事件--（int 角色id ，bool 是否上阵）
        /// </summary>
        E_Team,
    }
}


