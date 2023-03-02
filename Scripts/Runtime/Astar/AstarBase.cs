using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MapFrame
{
    public abstract class AstarBase
    {
        /// <summary>
        /// 直向移動一格
        /// </summary>
        protected float cost_upDown = 10;

        /// <summary>
        /// 橫向移動一格
        /// </summary>
        protected float cost_leftRight = 14;

        /// <summary>
        /// 斜向移動一格
        /// </summary>
        protected float cost_diagonal = 10;

        /// <summary>
        /// 最大步數
        /// </summary>
        protected int maxSteps = 1000;

        /// <summary>
        /// 待檢查的列表
        /// </summary>
        protected List<RoadNode> openList;

        /// <summary>
        /// 已檢查過的列表
        /// </summary>
        protected List<RoadNode> closeList;

        /// <summary>
        /// 開始節點
        /// </summary>
        protected RoadNode startNode;

        /// <summary>
        /// 當前節點
        /// </summary>
        protected RoadNode currentNode;

        /// <summary>
        /// 目標節點
        /// </summary>
        protected RoadNode targetNode;

        /// <summary>
        /// 地圖數據 
        /// </summary>
        public Dictionary<string, RoadNode> roadNodes;

        public bool optimize = false;

        public AstarBase()
        {
            this.openList = new List<RoadNode>();
            this.closeList = new List<RoadNode>();
            this.roadNodes = new Dictionary<string, RoadNode>();
        }

        public abstract RoadNode GetRoadNode(int dx, int dy, MapData md);
        public abstract Vector2 GetClick(float x, float y, MapData md);
        public abstract List<RoadNode> SeekPath(RoadNode _start, RoadNode _target);

        /// <summary>
        /// 設定節點的F值
        /// </summary>
        /// <param name="_node"> 輸入節點</param>
        protected void SetNodeF(RoadNode _node)
        {
            float g;

            if (_node.dx == this.currentNode.dx)
            {
                g = this.currentNode.g + this.cost_upDown;
            }
            else if (_node.dy == this.currentNode.dy)
            {
                g = this.currentNode.g + this.cost_leftRight;              
            }
            else
            {
                g = this.currentNode.g + this.cost_diagonal;
            }

            if (this.IsInOpenList(_node))
            {
                if (g < _node.g)
                {
                    _node.g = g;
                }
                else
                {
                    return;
                }
            }
            else
            {
                _node.g = g;
                //cc.log("長度: "+ g +" pos: "+_node.dx + " "+ _node.dy);
                this.openList.Add(_node);
                //cc.log(this.openList.length);
            }

            _node.parent = this.currentNode;
            _node.h = Mathf.Abs(this.targetNode.dx - _node.dx) + Mathf.Abs(this.targetNode.dy - _node.dy);
        }

        /// <summary>
        /// 是否在CloseList中
        /// </summary>
        /// <param name="_node">輸入節點</param>
        /// <returns></returns>
        protected bool IsInCloseList(RoadNode _node)
        {
            // 不存在回傳 -1 
            RoadNode find = this.closeList.Find((node) => node.dx == _node.dx && node.dy == _node.dy);

            return find != null;
        }

        /// <summary>
        /// 是否在OpenList中
        /// </summary>
        /// <param name="_node">輸入節點</param>
        /// <returns></returns>
        protected bool IsInOpenList(RoadNode _node)
        {
            // 不存在回傳 -1 
            RoadNode find = this.openList.Find((node) => node.dx == _node.dx && node.dy == _node.dy);

            return find != null;
        }

        /// <summary>
        /// 依照總和排序
        /// </summary>
        /// <param name="_node01">輸入節點1</param>
        /// /// <param name="_node02">輸入節點2</param>
        /// <returns></returns>
        protected int SortNode(RoadNode _node01, RoadNode _node02)
        {
            if (_node01.f < _node02.f)
            {
                return -1;
            }
            else if (_node01.f > _node02.f)
            {
                return 1;
            }
            return 0;
        }

        protected List<RoadNode> GetPath()
        {
            List<RoadNode> result = new List<RoadNode>();
            RoadNode _node = this.targetNode;

            while (_node != this.startNode)
            {
                result.Insert(0, _node);
                _node = _node.parent;
            }

            // 不將原點加入結果
            // result.Insert(0, this.startNode);

            if (this.optimize == false)
            {
                return result;
            }

            for (int i = 1; i < result.Count - 1; i++)
            {
                RoadNode preNode = result[i - 1];
                RoadNode midNode = result[i];
                RoadNode nextNode = result[i + 1];

                // 橫向
                bool boolH = this.CheckHorizontal(preNode, midNode,nextNode);

                // 直向
                bool boolV = this.CheckVertical(preNode, midNode, nextNode);

                bool boolM = this.CheckM(preNode, midNode, nextNode);

                if (boolH || boolV || boolM)
                {
                    result.RemoveAt(i);
                    i--;
                }

            }
            return result;
        }

        /// <summary>
        /// 檢查三個點是否在相同y點上
        /// </summary>
        /// <param name="preNode">1節點</param>
        /// <param name="midNode">2節點</param>
        /// <param name="nextNode">3節點</param>
        /// <returns></returns>
        protected bool CheckHorizontal(RoadNode preNode, RoadNode midNode, RoadNode nextNode)
        {
            float x1 = midNode.cx - preNode.cx;
            float x2 = nextNode.cx - midNode.cx;

            float y1 = midNode.cy - preNode.cy;
            float y2 = nextNode.cy - midNode.cy;

            return (x1 == x2) && (y1 == 0 && y2 == 0);
        }

        /// <summary>
        /// 檢查三個點是否在相同x點上
        /// </summary>
        /// <param name="preNode">1節點</param>
        /// <param name="midNode">2節點</param>
        /// <param name="nextNode">3節點</param>
        /// <returns></returns>
        protected bool CheckVertical(RoadNode preNode, RoadNode midNode, RoadNode nextNode)
        {
            float x1 = midNode.cx - preNode.cx;
            float x2 = nextNode.cx - midNode.cx;

            float y1 = midNode.cy - preNode.cy;
            float y2 = nextNode.cy - midNode.cy;

            return (y1 == y2) && (x1 == 0 && x2 == 0);
        }

        /// <summary>
        /// 檢查三個點是否在相同x點上
        /// </summary>
        /// <param name="preNode">1節點</param>
        /// <param name="midNode">2節點</param>
        /// <param name="nextNode">3節點</param>
        /// <returns></returns>
        protected bool CheckM(RoadNode preNode, RoadNode midNode, RoadNode nextNode)
        {
            float m1 = ((midNode.cx - preNode.cx) == 0)? 1 :(midNode.cy - preNode.cy) / (midNode.cx - preNode.cx);
            float m2 = ((nextNode.cx - midNode.cx) == 0)? 1 : (nextNode.cy - midNode.cy) / (nextNode.cx - midNode.cx);
            return (m1 == m2);
        }
    }
}

