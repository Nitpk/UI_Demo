/*
作者：阳贻凡
 */
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TFrameWork;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

namespace UIFramework
{
    //TODO 增加水平方向的列表

    /// <summary>
    /// 循环滚动列表
    /// </summary>
    /// <typeparam name="C">数据对应UI（单个格子）</typeparam>
    /// <typeparam name="D">数据</typeparam>
    public abstract class ViewList<C,D> : MonoBehaviour where C : MonoBehaviour
    {
        /// <summary>
        /// 列表滚动方向
        /// </summary>
        public enum E_Direction 
        {
            Horizontal,
            Vertical
        }

        [Tooltip("列表滚动方向")]
        [SerializeField]
        protected E_Direction direction;

        [Tooltip("UI格子")]
        [SerializeField]
        protected C cell;

        [Tooltip("列表的content组件")]
        [SerializeField]
        protected RectTransform content;

        //单个格子
        private RectTransform cellRectTransform;

        //单行格子数量
        [SerializeField]
        private int rowCellCount;
        //格子间隔
        [SerializeField]
        private Vector2 cellSpace;
        //格子尺寸
        public Vector2 CellSize => cellRectTransform.sizeDelta;
        //总尺寸
        public Vector2 TotalSize => CellSize + cellSpace;
        //背包锚点常量
        private readonly Vector2 contentAnchorMin = new Vector2(0, 1);
        private readonly Vector2 contentAnchorMax = new Vector2(1, 1);
        private readonly Vector2 cellPivot = new Vector2(0, 1);
        private readonly Vector2 cellAnchorMin = new Vector2(0, 1);
        private readonly Vector2 cellAnchorMax = new Vector2(0, 1);
        //背包行集合
        private readonly LinkedList<ViewCellBundle<C>> viewCellBundles = new LinkedList<ViewCellBundle<C>>();
        //当前行数
        public int RowCount
        {
            get
            {
                //数据数量
                int cellcount = Datas.Count;
                return cellcount % rowCellCount == 0 ? cellcount / rowCellCount : cellcount / rowCellCount + 1;
            }
        }
        //角色数据
        public List<D> Datas { get; private set; }
        //视野范围
        [SerializeField] 
        private RectTransform viewRange;

        /// <summary>
        /// 重置数据
        /// </summary>
        protected abstract void ResetCellData(C cell, D data, int dataIndex);

        protected virtual void Update()
        {
            if (Datas == null)
            {
                return;
            }
            UpdateDisplay();
        }
        /// <summary>
        /// 初始化列表
        /// </summary>
        /// <param name="datas"></param>
        /// <exception cref="System.Exception"></exception>
        public virtual void Initlize(List<D> datas,bool resetPos = false)
        {
            if (datas == null)
            {
                throw new System.Exception("角色数据为空");
            }
            cellRectTransform = cell.GetComponent<RectTransform>();
            Datas = datas;
            RecalculateContentSize(resetPos);
            //清除头部和尾部
            UpdateDisplay();
            RefrashViewRangeData();
        }

        /// <summary>
        /// 设置Content尺寸
        /// </summary>
        /// <param name="resetContentPos">是否让content归位</param>
        public void RecalculateContentSize(bool resetContentPos)
        {
            content.anchorMin = contentAnchorMin;
            content.anchorMax = contentAnchorMax;
            content.sizeDelta = new Vector2(content.sizeDelta.x, RowCount * TotalSize.y - cellSpace.y);
            
            if (resetContentPos)
            {
                content.anchoredPosition = Vector2.zero;
            }
        }
        /// <summary>
        /// 更新显示
        /// </summary>
        public void UpdateDisplay()
        {
            RemoveHead();
            RemoveTail();
            if (viewCellBundles.Count == 0)
            {
                RefreshAllCellInViewRange();
            }
            else
            {
                AddHead();
                AddTail();
            }
            //清除越界,清理在视野内的,在数据之外的UI
            RemoveItemOutOfListRange();
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        public void RefrashViewRangeData()
        {
            if (viewCellBundles.Count() == 0)
            {
                return;
            }
            LinkedListNode<ViewCellBundle<C>> curNode = viewCellBundles.First;
            bool flag = false;
            int count = 0;

            foreach (var bundle in viewCellBundles)
            {
                if (flag == true)
                {
                    break;
                }
                count++;
                int startIndex = bundle.index * rowCellCount;
                int endIndex = startIndex + bundle.Cells.Length - 1;

                //防止越界...
                if (endIndex >= Datas.Count)
                {
                    flag = true;
                    endIndex = Datas.Count - 1;
                }
                int j = 0;
                for (int i = startIndex; i <= endIndex && j < bundle.Cells.Length; i++, j++)
                {
                    //重置数据
                    ResetCellData(bundle.Cells[j], Datas.ElementAt(i), i);
                    
                }
                if (flag == true)
                {
                    while (j < bundle.Cells.Length)
                    {
                        try
                        {
                            bundle.Cells[j++].gameObject.SetActive(false);
                        }
                        catch (System.Exception)
                        {
                            throw;
                        }
                    }
                }
            }

            int remainCount = viewCellBundles.Count() - count;
            while (remainCount > 0)
            {
                remainCount--;
                //放回池中
                PoolMgr.Instance.PushPoolData(viewCellBundles.Last.Value);
                viewCellBundles.RemoveLast();
            }
        }
        /// <summary>
        /// 刷新界面内对应index的Cell的显示信息
        /// </summary>
        public void ElementAtDataChange(int index)
        {
            if (index < 0 || index >= Datas.Count)
            {
                Debug.LogError("指定行号["+index+"]越界");
                return;
            }
            if (viewCellBundles.Count == 0)
            {
                Debug.LogError("当前行数为0");
                return;
            }
            //最开头的行号
            int firstIndex = viewCellBundles.First.Value.index;
            //最后的行号
            int lastIndex = viewCellBundles.Last.Value.index;
            //目标的行号
            int targetIndex = index / rowCellCount;
            //对应的格子序号
            int cellIndex = index % rowCellCount;

            if (targetIndex >= firstIndex && targetIndex <= lastIndex)
            {
                C cell = viewCellBundles.Single((a) => a.index == targetIndex).Cells[cellIndex];

                //重置数据
                ResetCellData(cell, Datas.ElementAt(index), index);
                
            }
        }

        /// <summary>
        /// 刷新所有视野内的格子
        /// </summary>
        public void RefreshAllCellInViewRange()
        {
            //得到物品数量
            int rowCount = RowCount;
            //视野范围
            Vector2 viewRangeSize = viewRange.sizeDelta;
            //物品大小
            Vector2 totalSize = TotalSize;
            //格子大小
            Vector2 cellSize = CellSize;

            //开始的格子行号
            int startIndex = GetIndex(content.anchoredPosition.y);
            //最后的格子行号
            int endIndex = GetIndex(viewRangeSize.y + content.anchoredPosition.y);

            for (int i = startIndex; i <= endIndex && i < rowCount; i++)
            {
                Vector2 pos = new Vector2(content.anchoredPosition.x, -i * totalSize.y);
                var bundle = GetViewBundle(i, pos, cellSize, cellSpace);
                viewCellBundles.AddLast(bundle);
            }
            
        }

        // 添加头部
        private void AddHead()
        {
            //以头部元素向外计算出新头部的位置,计算该位置是否在显示区域，如果在显示区域则生成对应项目
            ViewCellBundle<C> bundle = viewCellBundles.First.Value;

            Vector2 offset = default;
            offset = new Vector2(0, TotalSize.y);

            Vector2 newHeadBundlePos = bundle.position + offset;

            while (OnViewRange(newHeadBundlePos))
            {
                int caculatedIndex = GetIndex(-newHeadBundlePos.y);
                int index = bundle.index - 1;

                if (index < 0) break;
                if (caculatedIndex != index)
                    Debug.LogError($"计算行号:{caculatedIndex}和计数行号{index}，计算出的行号和计数的行号不相等");

                bundle = GetViewBundle(index, newHeadBundlePos, CellSize, cellSpace);
                viewCellBundles.AddFirst(bundle);

                newHeadBundlePos = bundle.position + offset;
            }
        }
        // 移除头部
        private void RemoveHead()
        {
            if (viewCellBundles.Count == 0)
                return;

            ViewCellBundle<C> bundle = viewCellBundles.First.Value;
            while (AboveViewRange(bundle.position))
            {
                //放入池中
                PoolMgr.Instance.PushPoolData(bundle);
                viewCellBundles.RemoveFirst();

                if (viewCellBundles.Count == 0) break;

                bundle = viewCellBundles.First.Value;
            }
        }

        // 添加尾部
        private void AddTail()
        {
            //以尾部元素向外计算出新头部的位置,计算该位置是否在显示区域，如果在显示区域则生成对应项目
            ViewCellBundle<C> bundle = viewCellBundles.Last.Value;
            Vector2 offset = default;
            
            offset = new Vector2(0, -TotalSize.y);
            

            Vector2 newTailBundlePos = bundle.position + offset;

            while (OnViewRange(newTailBundlePos))
            {
                int caculatedIndex = GetIndex(-newTailBundlePos.y);
                int index = bundle.index + 1;

                if (index >= RowCount) break;
                if (caculatedIndex != index)
                    Debug.LogError($"计算行号:{caculatedIndex}和计数行号{index}，计算出的行号和计数的行号不相等");

                bundle = GetViewBundle(index, newTailBundlePos, CellSize, cellSpace);
                viewCellBundles.AddLast(bundle);

                newTailBundlePos = bundle.position + offset;
            }
        }

        /// <summary>
        /// 移除尾部
        /// </summary>
        private void RemoveTail()
        {
            if (viewCellBundles.Count == 0)
                return;

            ViewCellBundle<C> bundle = viewCellBundles.Last.Value;
            while (UnderViewRange(bundle.position))
            {
                //放入池中
                PoolMgr.Instance.PushPoolData(bundle);
                viewCellBundles.RemoveLast();

                if (viewCellBundles.Count == 0) break;

                bundle = viewCellBundles.Last.Value;
            }
        }

        // 得到格子行号
        private int GetIndex(float yPos)
        {
            int index = -1;
            index = Mathf.RoundToInt(yPos / TotalSize.y);
            return index;
        }
        //清空范围外的格子
        private void RemoveItemOutOfListRange()
        {
            if (viewCellBundles.Count() == 0)
                return;
            var bundle = viewCellBundles.Last.Value;
            int lastItemIndex = RowCount - 1;
            while (bundle.index > lastItemIndex && viewCellBundles.Count() > 0)
            {
                viewCellBundles.RemoveLast();
                //放回池中
                PoolMgr.Instance.PushPoolData(bundle);
            }
        }

        // 得到某行的格子集合
        private ViewCellBundle<C> GetViewBundle(int index, Vector2 postion, Vector2 cellSize, Vector2 cellSpace)
        {
            //从池子中取出
            ViewCellBundle<C> bundle = PoolMgr.Instance.GetPoolData<ViewCellBundle<C>>();
            //初始化格子
            if (!bundle.IsInit)
            {
                bundle.Init(rowCellCount);
                for (int j = 0; j < bundle.Cells.Length; j++)
                {
                    bundle.Cells[j] = Instantiate(cell, content);
                    bundle.Cells[j].gameObject.SetActive(false);
                }
            }
            //格子之间的偏移量
            Vector2 cellOffset = new Vector2(cellSize.x + cellSpace.x, 0);
            //设置格子位置和行号
            bundle.position = postion;
            bundle.index = index;
            int i = index * rowCellCount;
            int celllength = index * rowCellCount + bundle.Cells.Length;
            
            for (int j = 0; j < bundle.Cells.Length && i < celllength; j++, i++)
            {
                RectTransform rectTransform = bundle.Cells[j].GetComponent<RectTransform>();
                //重置位置
                rectTransform.pivot = cellPivot;
                rectTransform.anchorMin = cellAnchorMin;
                rectTransform.anchorMax = cellAnchorMax;
                rectTransform.anchoredPosition = postion + j * cellOffset;

                if (i < 0 || i >= Datas.Count)
                {
                    continue;
                }
                //重置数据
                ResetCellData(bundle.Cells[j], Datas.ElementAt(i), i);
            }
            
            return bundle;
        }

        //计算相对位置
        private Vector2 CaculateRelativePostion(Vector2 curPosition)
        {
            return new Vector2(curPosition.x, curPosition.y + content.anchoredPosition.y);
        }
        //范围外
        private bool AboveViewRange(Vector2 position)
        {
            Vector2 relativePos = CaculateRelativePostion(position);
            return relativePos.y > TotalSize.y;
        }

        private bool UnderViewRange(Vector2 position)
        {
            Vector2 relativePos = CaculateRelativePostion(position);
            return relativePos.y < -viewRange.sizeDelta.y;
        }

        private bool InViewRangeLeft(Vector2 position)
        {
            Vector2 relativePos = CaculateRelativePostion(position);
            return relativePos.x < -TotalSize.x;
        }

        private bool InViewRangeRight(Vector2 position)
        {
            Vector2 relativePos = CaculateRelativePostion(position);
            return relativePos.x > viewRange.sizeDelta.x;
        }

        //范围内
        private bool OnViewRange(Vector2 position)
        {
            return !AboveViewRange(position) && !UnderViewRange(position);
        }

    }
}