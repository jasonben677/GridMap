using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapFrame
{
    public class AstarFindRoadHoneycomb : AstarBase
    {
        /// <summary>
        /// 節點的相鄰節點()
        /// </summary>
        private int[,] _round;

        public AstarFindRoadHoneycomb() : base()
        {
            _round = new int[,] { { 1, 1 }, { 1, 0 }, { 0, -1 }, { -1, -1 }, { -1, 0 }, { 0, 1 }};
            this.cost_upDown = 1f;
            this.cost_leftRight = 1f;
            this.cost_diagonal = 1f;
        }
        public override RoadNode GetRoadNode(int dx, int dy, MapData md)
        {
            float width = (md.nodeWidth * 1.72f);
            float height = (md.nodeWidth * 1.5f);

            float newX = (dx - dy / 2) * width;
            float newY = dy * height;

            RoadNode newNode = new RoadNode();
            newNode.cx = newX;
            newNode.cy = newY;
            newNode.dx = dx;
            newNode.dy = dy;
            return newNode;
        }

        public override Vector2 GetClick(float x, float y, MapData md)
        {
            float width = (md.nodeWidth * 1.72f);
            float height = (md.nodeWidth * 1.5f);

            int cy = Mathf.FloorToInt(y / height);
            int cx = Mathf.FloorToInt((x + cy * width) / width);
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
            for (int i = 0; i < this._round.GetLength(0); i++)
            {
                float dx = _node.dx + this._round[i, 0];
                float dy = _node.dy + this._round[i, 1];

                this.roadNodes.TryGetValue(dx + "_" + dy, out RoadNode nodeGet);
                if (nodeGet != null && nodeGet != this.startNode && nodeGet.value != 1 && this.IsInCloseList(nodeGet) == false)
                {
                    this.SetNodeF(nodeGet);
                }
            }
        }
    }
}

