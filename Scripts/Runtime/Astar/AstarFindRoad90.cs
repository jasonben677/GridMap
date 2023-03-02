using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapFrame
{
    public class AstarFindRoad90 : AstarBase
    {
        /// <summary>
        /// 節點的相鄰的節點
        /// </summary>
        private int[,] _round;

        public AstarFindRoad90() : base()
        {
            this.cost_upDown = 1f;
            this.cost_leftRight = 1f;
            this.cost_diagonal = 1.4f;
            this._round = new int[,] { { 0, -1 }, { 1, -1 }, { 1, 0 }, { 1, 1 }, { 0, 1 }, { -1, 1 }, { -1, 0 }, { -1, -1 } };
        }
        public override RoadNode GetRoadNode(int dx, int dy, MapData md)
        {
            float originX = (md.nodeWidth / 2) - (md.camWidth / 2);
            float originY = (md.nodeHeight / 2) - (md.camHeight / 2);

            float newX = originX + dx * md.nodeWidth;

            float newY = originY + dy * md.nodeHeight;

            RoadNode newNode = new RoadNode();

            newNode.cx = newX;
            newNode.cy = newY;

            newNode.dx = dx;
            newNode.dy = dy;

            return newNode;
        }

        public override Vector2 GetClick(float x, float y, MapData md)
        {
            var cx = Mathf.FloorToInt(x / md.nodeWidth);
            var cy = Mathf.FloorToInt(y / md.nodeHeight);
            //Debug.Log("pos : " + cx + "，" + cy);
            return new Vector2(cx, cy);
        }


        public override List<RoadNode> SeekPath(RoadNode _start, RoadNode _target)
        {
            this.startNode = _start;
            this.currentNode = _start;
            this.targetNode = _target;

            foreach (var node in this.roadNodes)
            {
                node.Value.g = 0;
                node.Value.h = 0;
            }

            List<RoadNode> result = new List<RoadNode>();

            if (this.startNode == null || this.targetNode == null)
            {

                return result;
            }

            if (this.startNode.dx == this.targetNode.dx && this.startNode.dy == this.targetNode.dy)
            {
                Debug.Log("<color=#FEFF00>【A*】原點點擊</color>");
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
                    Debug.Log($"<color=#FEFF00>【A*】 => 超過最大次數並且沒找到目標網格, 共計算 <color=#00FFE1>{step}</color> 次</color>");
                    return this.SeekPath(this.startNode, closestNode);
                }
                step++;
                this.SearchRoundRoad(this.currentNode);

                if (this.openList.Count == 0)
                {
                    Debug.Log($"<color=#FEFF00>【A*】 => 沒找到目標網格, 共計算 <color=#00FFE1>{step}</color> 次</color>");
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
                    Debug.Log($"<color=#FEFF00>【A*】 => 找到目標網格, 共計算 <color=#00FFE1>{step}</color> 次</color>");
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
                RoadNode nodeGet = this.roadNodes[dx + "_" + dy];
                if (nodeGet != null && nodeGet != this.startNode && nodeGet.value != 1 && this.IsInCorner(nodeGet) == false && this.IsInCloseList(nodeGet) == false)
                {
                    this.SetNodeF(nodeGet);
                }
            }
        }

        /// <summary>
        /// 是否在角落中
        /// </summary>
        /// <param name="_node">輸入節點</param>
        /// <returns></returns>
        private bool IsInCorner(RoadNode _node)
        {
            if (_node.dx == this.currentNode.dx || _node.dy == this.currentNode.dy)
            {
                return false;
            }

            RoadNode node01 = this.roadNodes[_node.dx + "_" + this.currentNode.dy];
            RoadNode node02 = this.roadNodes[this.currentNode.dx + "_" + _node.dy];

            if (this.IsPassNode(node01) && this.IsPassNode(node02))
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// 是否可以通過
        /// </summary>
        /// <param name="_node">輸入節點</param>
        /// <returns></returns>
        private bool IsPassNode(RoadNode _node)
        {
            if (_node == null || _node.value == 1)
            {
                return false;
            }
            return true;
        }
    }
}

