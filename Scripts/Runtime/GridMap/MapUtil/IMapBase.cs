
using MapFrame;
using System;
using System.Collections.Generic;
using toinfiniityandbeyond.Tilemapping;
using UnityEngine;

namespace GridMap
{
    public abstract class IMapBase
    {
        /// <summary>
        /// 最終結果
        /// </summary>
        public MapData resultMap = new MapData();

        /// <summary>
        /// 計算用的地圖寬(左右各半個螢幕)
        /// </summary>
        public float mapCalWidth 
        {
            get 
            {
                return this.resultMap.mapWidth + this.resultMap.camWidth;
            }
        }

        /// <summary>
        /// 計算用的地圖高(上下各半個螢幕)
        /// </summary>
        public float mapCalHeight 
        {
            get
            {
                return this.resultMap.mapHeight + this.resultMap.camHeight;
            }
        }

        public Dictionary<string, RoadNode> nodeArr = new Dictionary<string, RoadNode>();
        public Dictionary<string, CanvasRenderer> nodeRender = new Dictionary<string, CanvasRenderer>();
        public Dictionary<string, Transform> nodeImg = new Dictionary<string, Transform>();

        public float originX = 0;
        public float originY = 0;

        public float countX = 0;
        public float countY = 0;

        public IMapBase()
        {

        }

        public abstract void OnLoad_NewMapdata();
        public abstract void OnLoad_ImportMapData(MapData mapData);
        public abstract Vector2 WorldToGrid(Vector3 mousePos);
        public abstract void CreateGrid(bool isNewNode = true);
        public abstract Vector2 GetClick(float x, float y);
        public abstract void DrawGrid(RoadNode node, LineRenderer render);

        /// <summary>
        /// 每個格子的mesh
        /// </summary>
        /// <returns></returns>
        public abstract Mesh DrawMesh();

        /// <summary>
        /// 整張地圖的mesh
        /// </summary>
        /// <returns></returns>
        public virtual Mesh[] CreateMapMesh(int minX, int maxX, int minY, int maxY)
        {
            return null;
        }

        public RoadNode CreateRoadNode(float x, float y, int value = 1)
        {
            RoadNode neNode = new RoadNode();
            Vector2 grid = this.WorldToGrid(new Vector3(x, y, 0));

            try
            {
                this.resultMap.roadDataArr[(int)grid.x, (int)grid.y] = value;

                neNode.dx = (int)grid.x;
                neNode.dy = (int)grid.y;
                neNode.cx = x;
                neNode.cy = y;
                neNode.value = value;
                neNode.drawOrder = -((grid.x + 1) + (grid.y + 1) * this.countX);

                this.nodeArr[grid.x + "_" + grid.y] = neNode;
            }
            catch (System.Exception)
            {
                Debug.LogError(grid.x+"_"+grid.y);
            }


            return neNode;
        }

        /// <summary>
        /// 計算斜邊長
        /// </summary>
        public void CalculateDiagonal()
        {
            float total = (this.resultMap.nodeWidth * this.resultMap.nodeWidth) + (this.resultMap.nodeHeight * this.resultMap.nodeHeight);
            float result = Mathf.Round(Mathf.Sqrt(total) * 100);
            this.resultMap.nodeDiagonal = result / 100f;
        }

        /// <summary>
        /// 變換成編輯器單位(預設公尺)
        /// </summary>
        public void ChangeUnitToM()
        {
            this.resultMap.mapWidth /= MapEditorDefine.scale;
            this.resultMap.mapHeight /= MapEditorDefine.scale;

            this.resultMap.nodeWidth /= MapEditorDefine.scale;
            this.resultMap.nodeHeight /= MapEditorDefine.scale;

            this.resultMap.nodeDiagonal /= MapEditorDefine.scale;

            this.resultMap.camWidth /= MapEditorDefine.scale;
            this.resultMap.camHeight /= MapEditorDefine.scale;
        }

        /// <summary>
        /// 變換成公分
        /// </summary>
        public void ChangeUnitToCM()
        {
            this.resultMap.mapWidth *= MapEditorDefine.scale;
            this.resultMap.mapHeight *= MapEditorDefine.scale;

            this.resultMap.nodeWidth *= MapEditorDefine.scale;
            this.resultMap.nodeHeight *= MapEditorDefine.scale;

            this.resultMap.nodeDiagonal *= MapEditorDefine.scale;

            this.resultMap.camWidth *= MapEditorDefine.scale;
            this.resultMap.camHeight *= MapEditorDefine.scale;
        }
    }
}


