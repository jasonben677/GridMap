#if UNITY_EDITOR
using MapFrame;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace GridMap
{
    public class RuntimeMapHexagonal : IMapBase
    {
        public override void OnLoad_NewMapdata()
        {
            this.originX = (this.resultMap.nodeWidth / 2) - (this.resultMap.camWidth / 2);
            this.originY = (this.resultMap.nodeHeight / 2) - (this.resultMap.camHeight / 2);

            this.countX = Mathf.FloorToInt(this.mapCalWidth / this.resultMap.nodeWidth);
            this.countY = Mathf.FloorToInt(this.mapCalHeight / (this.resultMap.nodeHeight * 0.75f));

            this.resultMap.sizeX = this.countX;
            this.resultMap.sizeY = this.countY;
            this.resultMap.roadDataArr = new int[(int)this.countX, (int)this.countY];

            float xPadding = this.resultMap.camWidth / this.resultMap.nodeWidth;
            float yPadding = this.resultMap.camHeight / this.resultMap.nodeHeight;
            Debug.Log($"<color=#00FFFF>Grid Padding Num: ({xPadding}, {yPadding});</color>");
            Debug.Log($"<color=#00FF86>Map Grid Num: (x => {countX - xPadding}, y => {countY - yPadding});</color>");
            Debug.Log($"<color=#FFE506>Total Sum of Grid Num: (x => {countX }, y => {countY})</color>");
        }
        public override void OnLoad_ImportMapData(MapData mapData)
        {
            this.resultMap = mapData;
            this.originX = (this.resultMap.nodeWidth / 2) - (this.resultMap.camWidth / 2);
            this.originY = (this.resultMap.nodeHeight / 2) - (this.resultMap.camHeight / 2);

            this.countX = mapData.sizeX;
            this.countY = mapData.sizeY;

            float xPadding = this.resultMap.camWidth / this.resultMap.nodeWidth;
            float yPadding = this.resultMap.camHeight / this.resultMap.nodeHeight;
            Debug.Log($"<color=#00FFFF>Grid Padding Num: ({xPadding}, {yPadding});</color>");
            Debug.Log($"<color=#00FF86>Map Grid Num: (x => {countX - xPadding}, y => {countY - yPadding});</color>");
            Debug.Log($"<color=#FFE506>Total Sum of Grid Num: (x => {countX }, y => {countY})</color>");
        }

        public override Vector2 WorldToGrid(Vector3 mousePos)
        {
            float posX = mousePos.x + this.resultMap.camWidth / 2;
            float posY = mousePos.y + this.resultMap.camHeight / 2;
            return this.GetClick(posX, posY);
        }
        public override void CreateGrid(bool isNewNode = true)
        {
            for (int y = 0; y < this.countY; y++)
            {
                this.originY = (this.resultMap.nodeHeight - this.resultMap.camHeight) / 2;
                this.originY += y * (this.resultMap.nodeHeight * 0.75f);
                float addX = (y % 2 == 0) ? 0 : this.resultMap.nodeWidth / 2;

                for (int x = 0; x < this.countX; x++)
                {
                    this.originX = (this.resultMap.nodeHeight - this.resultMap.camWidth) / 2;
                    this.originX += this.resultMap.nodeWidth * x;
                    this.originX += addX;
                    RoadNode node = CreateRoadNode(this.originX, this.originY);
                }

            }
        }
        public override void DrawGrid(RoadNode node, LineRenderer renderer)
        {

        }
        public override Vector2 GetClick(float x, float y)
        {

            float halfWidth = this.resultMap.nodeWidth / 2f;
            float halfHeight = this.resultMap.nodeHeight / 2f;

            float cy = Mathf.Clamp(y - halfHeight, 0, int.MaxValue);
            float cx = Mathf.Clamp(x - halfWidth, 0, int.MaxValue);

            float dy = Mathf.Round(cy / (this.resultMap.nodeHeight * 0.75f));
            float dx;

            if (dy % 2 == 0)
            {
                dx = Mathf.Round(cx / this.resultMap.nodeWidth);
            }
            else
            {
                dx = Mathf.Round((cx - halfWidth) / this.resultMap.nodeWidth);
            }
            //Debug.LogError("input : " + x + "_" + y);
            //Debug.LogError("output : " + dx + "_" + dy);
            return new Vector2(dx, dy);
        }

        public override Mesh[] CreateMapMesh(int minX, int maxX, int minY, int maxY)
        {
            List<MeshData> meshDatas = new List<MeshData>();

            float halfWidth = this.resultMap.nodeWidth / 2f;
            float quarterHeight = this.resultMap.nodeHeight / 4f;


            List<Vector3> vertices = new List<Vector3>();

            List<int> lines = new List<int>();


            // 預設第一個Mesh
            meshDatas.Add(new MeshData());


            for (int y = minY; y < maxY; y++)
            {
                for (int x = minX; x < maxX; x++)
                {
                    MeshData data = meshDatas[meshDatas.Count - 1];

                    if (data.vertices.Count >= 65000)
                    {
                        meshDatas.Add(new MeshData());
                        data = meshDatas[meshDatas.Count - 1];
                    }

                    this.nodeArr.TryGetValue(x + "_" + y, out RoadNode road);
                    if (road == null) return new Mesh[] { };

                    float2 node = new float2(road.cx, road.cy);

                    data.vertices.Add(new Vector3(node.x - halfWidth, node.y + quarterHeight));
                    data.vertices.Add(new Vector3(node.x, node.y + quarterHeight * 2));
                    data.vertices.Add(new Vector3(node.x + halfWidth, node.y + quarterHeight));
                    data.vertices.Add(new Vector3(node.x + halfWidth, node.y - quarterHeight));
                    data.vertices.Add(new Vector3(node.x, node.y - quarterHeight * 2));
                    data.vertices.Add(new Vector3(node.x - halfWidth, node.y - quarterHeight));

                    int index = data.vertices.Count - 6;

                    data.lines.Add(index);
                    data.lines.Add(index + 1);

                    data.lines.Add(index + 1);
                    data.lines.Add(index + 2);

                    data.lines.Add(index + 2);
                    data.lines.Add(index + 3);

                    data.lines.Add(index + 3);
                    data.lines.Add(index + 4);

                    data.lines.Add(index + 4);
                    data.lines.Add(index + 5);

                    data.lines.Add(index + 5);
                    data.lines.Add(index + 0);

                }
            }

            List<Mesh> output = new List<Mesh>();

            for (int i = 0; i < meshDatas.Count; i++)
            {
                Mesh mesh = new Mesh();
                mesh.vertices = meshDatas[i].vertices.ToArray();
                mesh.SetIndices(meshDatas[i].lines, MeshTopology.Lines, 0);
                output.Add(mesh);
            }

            return output.ToArray();
        }

        public override Mesh DrawMesh()
        {
            Mesh mesh = new Mesh();

            float width = this.resultMap.nodeWidth * 0.8f;
            float height = this.resultMap.nodeHeight * 0.8f;

            Vector3[] vertices = new Vector3[6];
            Vector2[] uv = new Vector2[6];
            int[] triangles = new int[3 * 4];

            vertices[0] = new Vector3(-(width / 2), (height / 4));
            vertices[1] = new Vector3(0, (height / 2));
            vertices[2] = new Vector3((width / 2), (height / 4));
            vertices[3] = new Vector3((width / 2), -(height / 4));
            vertices[4] = new Vector3(0, -(height / 2));
            vertices[5] = new Vector3(-(width / 2), -(height / 4));

            uv[0] = new Vector2(0, 0.75f);
            uv[1] = new Vector2(0.5f, 1);
            uv[2] = new Vector2(1, 0.75f);
            uv[3] = new Vector2(1, 0.25f);
            uv[4] = new Vector2(0.5f, 0);
            uv[5] = new Vector2(0, 0.25f);


            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 2;
            triangles[3] = 2;
            triangles[4] = 3;
            triangles[5] = 4;
            triangles[6] = 4;
            triangles[7] = 5;
            triangles[8] = 0;
            triangles[9] = 0;
            triangles[10] = 2;
            triangles[11] = 4;

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;

            return mesh;
        }
    }
}
#endif
