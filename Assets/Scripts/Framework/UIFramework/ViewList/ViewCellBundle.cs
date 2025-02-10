/*
 * 作者：阳贻凡
 */
using System.Collections;
using TFrameWork;
using UnityEngine;

namespace UIFramework
{
    /// <summary>
    /// 格子组合
    /// </summary>
    /// <typeparam name="TCell">格子UI</typeparam>
    public class ViewCellBundle<TCell> : IPoolData 
        where TCell : MonoBehaviour
    {
        /// <summary>
        /// 行号
        /// </summary>
        public int index;
        /// <summary>
        /// 位置
        /// </summary>
        public Vector2 position;
        /// <summary>
        /// 格子集合
        /// </summary>
        public TCell[] Cells { get; private set; }
        /// <summary>
        /// 格子容量
        /// </summary>
        public int CellCapacity => Cells.Length;
        /// <summary>
        /// 是否初始化
        /// </summary>
        public bool IsInit { get; private set; }
        public ViewCellBundle() { IsInit = false; }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="gameObjectCapacity"></param>
        public void Init(int gameObjectCapacity)
        {
            if (IsInit) return;

            Cells = new TCell[gameObjectCapacity];

            IsInit = true;
        }

        public void ResetData()
        {
            //隐藏所有格子
            index = -1;
            foreach (var cell in Cells)
            {
                if (cell != null)
                {
                    cell.gameObject.SetActive(false);
                }
            }
        }


    }
}