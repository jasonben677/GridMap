using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapFrame
{
    public class AstarFindRoad45 : AstarBase
    {
        /// <summary>
        /// 節點的相鄰的節點()
        /// </summary>
        private int[,] _round01;
        private int[,] _round02;

        public AstarFindRoad45() : base()
        {
            _round01 = new int[,] { { 0, -2 }, { 1, -1 }, { 1, 0 }, { 1, 1 }, { 0, 2}, { 0, 1 }, { -1, 0 }, { 0, -1 }  };
            _round02 = new int[,] { { 0, -2 }, { 0, -1 }, { 1, 0 }, { 0, 1 }, { 0, 2}, { -1, 1 }, { -1, 0 }, { -1, -1 }  };
        }
        public override RoadNode GetRoadNode(int dx, int dy, MapData md)
        {
            float halfwidth = Mathf.Floor(md.nodeWidth / 2);
            float halfheight = Mathf.Floor(md.nodeHeight / 2);

            float newX = halfwidth + (dx * md.nodeWidth) + (dy % 2) * halfwidth;
            float newY = halfheight + halfheight + (dy * halfheight);

            RoadNode newNode = new RoadNode();
            newNode.cx = newX - md.nodeWidth;
            newNode.cy = newY - md.nodeHeight;
            newNode.dx = dx;
            newNode.dy = dy;
            return newNode;
        }

        public override Vector2 GetClick(float x, float y, MapData md)
        {
            float halfWidth = md.nodeWidth / 2;
            float halfHeight = md.nodeHeight / 2;

            int cy = Mathf.RoundToInt((y - halfHeight) / halfHeight);
            int cx = Mathf.RoundToInt((x - (halfWidth + cy % 2 * halfWidth)) / md.nodeWidth);
            return new Vector2(cx, cy);
        }


        public override List<RoadNode> SeekPath(RoadNode _start, RoadNode _target)
        {
            this.startNode = _start;
            this.currentNode = _start;
            this.targetNode = _target;

            List<RoadNode> result = new List<RoadNode>();

            if (this.startNode == null || this.targetNode == null)
            {

                return result;
            }

            if (this.startNode.dx == this.targetNode.dx && this.startNode.dy == this.targetNode.dy)
            {
                Debug.Log("!!! 原點點擊");
                result.Add(this.startNode);
                return result;
            }

            this.openList = new List<RoadNode>();
            this.closeList = new List<RoadNode>();

            int step = 0;
            RoadNode closestNode = null;
            while (true)
            {
                if (step > this.maxSteps)
                {
                    Debug.Log("!!! 超過最大次數 沒找到目標，共計算" + step + "次");
                    return this.SeekPath(this.startNode, closestNode);
                }
                step++;
                this.SearchRoundRoad(this.currentNode);

                if (this.openList.Count == 0)
                {
                    Debug.Log("!!! 沒找到目標，共計算" + step + "次");
                    return this.SeekPath(this.startNode, closestNode);
                }

                this.openList.Sort(this.SortNode);
                this.currentNode = this.openList[0];
                this.openList.RemoveAt(0);

                if (closestNode == null)
                {
                    closestNode = this.currentNode;
                }
                else
                {
                    if (this.currentNode.h < closestNode.h)
                    {
                        closestNode = this.currentNode;
                    }
                }

                if (this.currentNode == this.targetNode)
                {
                    Debug.Log("!!! 找到目標，共計算" + step + "次");
                    return this.GetPath();
                }
                else
                {
                    this.closeList.Add(this.currentNode);
                }
            }
        }

        /// <summary>
        /// 找尋輸入節點的鄰居
        /// </summary>
        /// <param name="currentNode">輸入節點</param>
        private void SearchRoundRoad(RoadNode _node)
        {
            for (int i = 0; i < this._round01.GetLength(0); i++)
            {
                float dx;                
                float dy;

                if ((_node.dy % 2) == 0)
                {
                    dx = _node.dx + this._round02[i, 0];
                    dy = _node.dy + this._round02[i, 1];
                }
                else
                {
                    dx = _node.dx + this._round01[i, 0];
                    dy = _node.dy + this._round01[i, 1];
                }

                this.roadNodes.TryGetValue(dx + "_" + dy, out RoadNode nodeGet);
                if (nodeGet != null && nodeGet != this.startNode && nodeGet.value != 1 && this.IsInCloseList(nodeGet) == false)
                {
                    this.SetNodeF(nodeGet);
                }
            }
        }
    }
}

