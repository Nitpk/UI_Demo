/*
 * 作者：阳贻凡
 */
using System.Collections;
using UnityEngine;
using UIFramework;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;
using System.Linq;

namespace Demo
{
    /// <summary>
    /// 角色列表
    /// </summary>
    public class CharacterViewList : ViewList<UICharacterCell, CharacterInfo>
    {
        protected override void ResetCellData(UICharacterCell cell, CharacterInfo data, int dataIndex)
        {
            cell.gameObject.SetActive(true);
            cell.UpdateUIDisplay(data);
        }
    }
}