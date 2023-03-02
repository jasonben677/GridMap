using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapFrame
{
    public class JPSFindRoad90 : AstarBase
    {
        /// <summary>
        /// 節點的相鄰的節點
        /// </summary>
        private int[,] _round;

        public JPSFindRoad90()
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
            //this.roadNodes.TryGetValue("39_35", out RoadNode testStartNode);
            //this.roadNodes.TryGetValue("42_41", out RoadNode testTargetNode);

            //if (testStartNode == null) return new List<RoadNode>();
            //if (testTargetNode == null) return new List<RoadNode>();

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

            Debug.LogError("start : " + this.startNode.dx + "_" + this.startNode.dy);
            Debug.LogError("end : " + this.targetNode.dx + "_" + this.targetNode.dy);
            Debug.LogError("targetNode.value : " + targetNode.value);

            if (targetNode.value == 1) return result;

            this.openList = new List<RoadNode>();
            this.closeList = new List<RoadNode>();

            this.openList.Add(new RoadNode(_start, null));

            while (this.openList.Count > 0)
            {
                int nodeIndex = 0;

                for (int i = 0; i < this.openList.Count; i++)
                {
                    if (this.openList[i].f < this.openList[nodeIndex].f)
                    {
                        nodeIndex = i;
                    } 
                }

                this.currentNode = this.openList[nodeIndex];

                this.openList.Remove(this.currentNode);
                this.closeList.Add(this.currentNode);

                if (this.currentNode.dx == this.targetNode.dx && this.currentNode.dy == this.targetNode.dy)
                {
                    while (this.currentNode != null)
                    {
                        result.Add(this.currentNode);
                        this.currentNode = this.currentNode.parent;
                    }
                    result.Reverse();
                    break;
                }

                this._IdentifySuccessors(this.currentNode);
            }

            Debug.LogError("---------");
            foreach (var node in result)
            {
                Debug.LogError("node : " + node.dx + "_" + node.dy);
            }
            Debug.LogError("---------");

            return result;
        }

        private void _IdentifySuccessors(RoadNode node)
        {
            List<Vector2Int> neighborPoints = this._FindNeighbors(node);

            for (int i = 0; i < neighborPoints.Count; i++)
            {
                this._Jump(node, neighborPoints[i].x, neighborPoints[i].y);
            }

        }

        private void _Jump(RoadNode nowNode, int dirX, int dirY)
        {
            this.roadNodes.TryGetValue((nowNode.dx + dirX) + "_" + (nowNode.dy + dirY), out RoadNode next);

            if (next == null) return;

            if (dirX != 0 && dirY != 0)
            {
                next.g = nowNode.g + this.cost_diagonal;

                if (next.dx == targetNode.dx && next.dy == targetNode.dy)
                {
                    this._AddToOpenList(next.dx, next.dy, next.g, this.currentNode);
                    return;
                }

                if (next.value == 1) return;

                bool checkX = !this._Walkable(nowNode.dx + dirX, nowNode.dy);
                bool checkY = !this._Walkable(nowNode.dx, nowNode.dy + dirY);

                if (checkX || checkY)
                {
                    this._AddToOpenList(next.dx, next.dy, next.g, this.currentNode);
                    return;
                }

                this._Jump(next, dirX, 0);
                this._Jump(next, 0, dirY);
            }
            else if (dirX != 0 && dirY == 0)
            {
                next.g = nowNode.g + this.cost_leftRight;

                if (next.dx == targetNode.dx && next.dy == targetNode.dy)
                {
                    this._AddToOpenList(next.dx, next.dy, next.g, this.currentNode);
                    return;
                }

                if (next.value == 1)
                {
                    //       -- J
                    //     /     
                    //    S --- X
                    //     \     
                    //       -- J

                    bool checkUp = this._Walkable(nowNode.dx + dirX, nowNode.dy + 1);
                    bool checkDown = this._Walkable(nowNode.dx + dirX, nowNode.dy - 1);
                    if (checkUp) 
                    {
                        this._AddToOpenList(nowNode.dx + dirX, nowNode.dy + 1, nowNode.g + this.cost_diagonal, this.currentNode);
                    }

                    if (checkDown) 
                    {
                        this._AddToOpenList(nowNode.dx + dirX, nowNode.dy - 1, nowNode.g + this.cost_diagonal, this.currentNode);
                    }

                    return;
                }
                else
                {
                    //    X    J
                    //    |    | 
                    //    S -- O
                    //    |    | 
                    //    X    J
                    bool checkUpDiagonal = this._Walkable(next.dx, next.dy + 1) && !this._Walkable(nowNode.dx, nowNode.dy + 1);
                    bool checkDownDiagonal = this._Walkable(next.dx, next.dy - 1) && !this._Walkable(nowNode.dx, nowNode.dy - 1);

                    if (checkUpDiagonal || checkDownDiagonal)
                    {
                        this._AddToOpenList(nowNode.dx, nowNode.dy, nowNode.g, this.currentNode);
                    }                  
                }

            }
            else if (dirX == 0 && dirY != 0)
            {
                next.g = nowNode.g + this.cost_upDown;

                if (next.dx == targetNode.dx && next.dy == targetNode.dy)
                {
                    this._AddToOpenList(next.dx, next.dy, next.g, this.currentNode);
                    return;
                }

                if (next.value == 1)
                {
                    //     J  X  J 
                    //      \ | /
                    //        S 

                    bool checkLeft = this._Walkable(nowNode.dx - 1, nowNode.dy + dirY);
                    bool checkRight = this._Walkable(nowNode.dx + 1, nowNode.dy + dirY);
                    if (checkLeft) 
                    {
                        this._AddToOpenList(nowNode.dx - 1, nowNode.dy + dirY, nowNode.g + this.cost_diagonal, this.currentNode);
                    }

                    if (checkRight) 
                    {
                        this._AddToOpenList(nowNode.dx + 1, nowNode.dy + dirY, nowNode.g + this.cost_diagonal, this.currentNode);
                    }
                    return;
                }
                else
                {
                    //     J   O   J
                    //       \ | /
                    //     X - S - X 

                    bool checkLeftDiagonal = this._Walkable(next.dx - 1, next.dy) && !this._Walkable(nowNode.dx - 1, nowNode.dy);
                    bool checkRightDiagonal = this._Walkable(next.dx + 1, next.dy) && !this._Walkable(nowNode.dx + 1, nowNode.dy);

                    if (checkLeftDiagonal || checkRightDiagonal)
                    {
                        this._AddToOpenList(nowNode.dx, nowNode.dy, nowNode.g, this.currentNode);
                    }                
                }

            }

            this._Jump(next, dirX, dirY);
        }

        private void _AddToOpenList(int x, int y, float gValue, RoadNode parent)
        {
            this.roadNodes.TryGetValue(x + "_" + y, out RoadNode node);
            if (node == null) return;

            bool inOpenList = this.IsInOpenList(node);
            bool inCloseList = this.IsInCloseList(node);

            if (!inOpenList && !inCloseList) 
            {
                node.g = gValue;
                node.h = this._ToTargetLength(node);
                node.parent = parent;
                this.openList.Add(node);
            }
            
        }

        private List<Vector2Int> _FindNeighbors(RoadNode node)
        {
            List<Vector2Int> list = new List<Vector2Int>();

            List<Vector2Int> block = new List<Vector2Int>();

            if (node.parent != null)
            {
                int dx = Mathf.Clamp(node.parent.dx - node.dx, -1, 1);
                int dy = Mathf.Clamp(node.parent.dy - node.dy, -1, 1);

                block.Add(new Vector2Int(dx, dy));

                if (dx == 0)
                {
                    block.Add(new Vector2Int(1, dy));
                    block.Add(new Vector2Int(-1, dy));
                }
                if (dy == 0)
                {
                    block.Add(new Vector2Int(dx, 1));
                    block.Add(new Vector2Int(dx, -1));
                }
            }


            List<Vector2Int> cellNeighbors = this._GetNeighbor();
            for (int i = 0; i < cellNeighbors.Count; i++)
            {
                bool canUse = true;

                foreach (var v2 in block)
                {
                    if (cellNeighbors[i].x == v2.x && cellNeighbors[i].y == v2.y)
                    {
                        canUse = false;
                        break;
                    }
                }

                if(canUse) list.Add(cellNeighbors[i]);
            }

            return list;

        }

        /// <summary>
        /// 找尋輸入節點的鄰居
        /// </summary>
        /// <param name="currentNode">輸入節點</param>
        private List<Vector2Int> _GetNeighbor()
        {
            List<Vector2Int> result = new List<Vector2Int>();

            for (int i = 0; i < this._round.GetLength(0); i++)
            {
                result.Add(new Vector2Int(this._round[i, 0], this._round[i, 1]));
            }

            return result;
        }

        private bool _Walkable(int x, int y)
        {
            this.roadNodes.TryGetValue(x + "_" + y, out RoadNode node);

            if (node == null) return false;

            return node.value == 0;
        }

        private float _ToTargetLength(RoadNode current)
        {
            return Mathf.Abs(this.targetNode.dx - current.dx) + Mathf.Abs(this.targetNode.dy - current.dy);
        }

        private List<RoadNode> _FillPath(List<RoadNode> path)
        {
            if (path.Count == 0)
                return path;

            List<RoadNode> newPath = new List<RoadNode>();

            newPath.Add(path[0]);

            for (int i = 0; i < path.Count -1 ; i++)
            {
                int dx = Mathf.Clamp(path[i + 1].dx - path[i].dx, -1, 1);
                int dy = Mathf.Clamp(path[i + 1].dy - path[i].dy, -1, 1);

                while (newPath[newPath.Count - 1] != path[i + 1])
                {
                    Vector2Int position = new Vector2Int(newPath[newPath.Count - 1].dx, newPath[newPath.Count - 1].dy);
                    this.roadNodes.TryGetValue((position.x + dx) + "_" +(position.y + dy), out RoadNode newAdd);
                    if (newAdd != null) newPath.Add(newAdd);
                }          
            }
            return newPath;
        }

    }
}

