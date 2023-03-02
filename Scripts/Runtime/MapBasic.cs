using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MapFrame
{
    public abstract class MapBasic
    {
        public MapData mapData = new MapData();
        public AstarBase astarTypeUse = null;
        public Dictionary<string, RoadNode> nodeList = new Dictionary<string, RoadNode>();

        public MapBasic()
        {
            this.nodeList = new Dictionary<string, RoadNode>();
        }

        /// <summary>
        /// 重置地圖資料
        /// </summary>
        public virtual void ResetMap()
        {
            this.mapData = null;
            this.nodeList.Clear();
            this.astarTypeUse = null;
        }

        /// <summary>
        /// 取得周圍9宮格節點(90度)
        /// </summary>
        /// <param name="nodeData"></param>
        /// <returns></returns>
        public string[] GetNeighborNode90(RoadNode nodeData)
        {
            string[] result = new string[9];
            result[0] = (nodeData.dx - 1) + "_" + (nodeData.dy + 1);
            result[1] = nodeData.dx + "_" + (nodeData.dy + 1);
            result[2] = (nodeData.dx + 1) + "_" + (nodeData.dy + 1);
            result[3] = (nodeData.dx - 1) + "_" + nodeData.dy;
            result[4] = nodeData.dx + "_" + nodeData.dy;
            result[5] = (nodeData.dx + 1) + "_" + nodeData.dy;
            result[6] = (nodeData.dx - 1) + "_" + (nodeData.dy - 1);
            result[7] = nodeData.dx + "_" + (nodeData.dy - 1);
            result[8] = (nodeData.dx + 1) + "_" + (nodeData.dy - 1);
            return result;
        }

        /// <summary>
        /// 取得周圍9宮格節點(45度)
        /// </summary>
        /// <param name="nodeData"></param>
        /// <returns></returns>
        public string[] GetNeighborNode45(RoadNode nodeData)
        {
            string[] result = new string[8];
            result[0] = (nodeData.dx) + "_" + (nodeData.dy - 2);
            result[1] = nodeData.dx + "_" + (nodeData.dy - 1);
            result[2] = (nodeData.dx + 1) + "_" + nodeData.dy;
            result[3] = nodeData.dx + "_" + (nodeData.dy + 1);
            result[4] = nodeData.dx + "_" + (nodeData.dy + 2);
            result[5] = (nodeData.dx - 1) + "_" + (nodeData.dy + 1);
            result[6] = (nodeData.dx - 1) + "_" + nodeData.dy;
            result[7] = (nodeData.dx - 1) + "_" + (nodeData.dy - 1);

            if ((nodeData.dy % 2) == 0)
            {
                result[1] = nodeData.dx + "_" + (nodeData.dy - 1);
                result[3] = nodeData.dx + "_" + (nodeData.dy + 1);
                result[5] = (nodeData.dx - 1) + "_" + (nodeData.dy + 1);
                result[7] = (nodeData.dx - 1) + "_" + (nodeData.dy - 1);
            }
            else
            {
                result[1] = (nodeData.dx + 1) + "_" + (nodeData.dy - 1);
                result[3] = (nodeData.dx + 1) + "_" + (nodeData.dy + 1);
                result[5] = nodeData.dx + "_" + (nodeData.dy + 1);
                result[7] = nodeData.dx + "_" + (nodeData.dy - 1);
            }
            return result;
        }

        /// <summary>
        /// 取得周圍9宮格節點(蜂巢式)
        /// </summary>
        /// <param name="nodeData"></param>
        /// <returns></returns>
        public string[] GetNeighborNodeHoneyComb(RoadNode nodeData)
        {
            string[] result = new string[6];
            result[0] = (nodeData.dx + 1) + "_" + (nodeData.dy + 1);
            result[1] = (nodeData.dx + 1) + "_" + nodeData.dy;
            result[2] = nodeData.dx + "_" + (nodeData.dy - 1);
            result[3] = (nodeData.dx - 1) + "_" + (nodeData.dy - 1);
            result[4] = (nodeData.dx - 1) + "_" + nodeData.dy;
            result[5] = nodeData.dx + "_" + (nodeData.dy + 1);
            return result;
        }

        public RoadNode GetNodeFromGrid(int x, int y)
        {
            string gridKey = $"{x}_{y}";
            return this.nodeList[gridKey];
        }

        public RoadNode GetNodeFromGrid(string gridKey)
        {
            return this.nodeList[gridKey];
        }

        /// <summary>
        /// 轉換成節點
        /// </summary>
        /// <param name="posX">X座標</param>
        /// <param name="posY">Y座標</param>
        /// <returns></returns>
        public RoadNode GetNodeFromWorld(float posX, float posY)
        {
            float nextX = Mathf.Max(0, posX + (this.mapData.camWidth / 2));
            float nextY = Mathf.Max(0, posY + (this.mapData.camHeight / 2));

            Vector2 resultPos = this.astarTypeUse.GetClick(nextX, nextY, this.mapData);
            RoadNode result = this.nodeList[resultPos.x + "_" + resultPos.y];

            return result;
        }

        /// <summary>
        /// 加載完地圖JSON網格數據後, 進行設置網格節點資料 (二維陣列 [x, y])
        /// </summary>
        /// <param name="mapData"></param>
        /// <returns></returns>
        public async UniTask SetGridData(MapData mapData)
        {
            if (mapData == null) return;

            int xLen = mapData.roadDataArr.GetLength(0);
            int yLen = mapData.roadDataArr.GetLength(1);

            for (int x = 0; x < xLen; x++)
            {
                for (int y = 0; y < yLen; y++)
                {
                    int dx = x;
                    int dy = y;

                    int value = mapData.roadDataArr[x, y];
                    RoadNode dataNode = this.astarTypeUse.GetRoadNode(dx, dy, mapData);

                    if (dataNode == null)
                    {
                        Debug.Log(string.Format("<color=#FF0000>ERROR x-Node: {0}</color>", dx));
                        Debug.Log(string.Format("<color=#FF0000>ERROR y-Node: {0}</color>", dy));
                        continue;
                    }

                    dataNode.drawOrder = ((dx + 1) + (dy + 1) * mapData.roadDataArr.GetLength(1)) * -1;
                    dataNode.value = value;
                    this.nodeList[$"{dx}_{dy}"] = dataNode;
                }
            }

            this.astarTypeUse.roadNodes = this.nodeList;

            await UniTask.Yield();
        }
    }
}

