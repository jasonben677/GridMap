#if UNITY_EDITOR
using MapFrame;
using UnityEditor;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using System.Collections.Generic;

namespace GridMap
{
    public class RuntimeMap90 : IMapBase
    {
        public RuntimeMap90()
        {

        }

        public override void OnLoad_NewMapdata()
        {
            this.originX = (this.resultMap.nodeWidth / 2) - (this.resultMap.camWidth / 2);
            this.originY = (this.resultMap.nodeHeight / 2) - (this.resultMap.camHeight / 2);

            this.countX = Mathf.FloorToInt(this.mapCalWidth / this.resultMap.nodeWidth);
            this.countY = Mathf.FloorToInt(this.mapCalHeight / this.resultMap.nodeHeight);

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
            for (int x = 0; x < this.countX; x++)
            {
                this.originY = (this.resultMap.nodeHeight / 2) - (this.resultMap.camHeight / 2);

                for (int y = 0; y < this.countY; y++)
                {
                    int value = isNewNode ? 1 : this.resultMap.roadDataArr[x, y];
                    RoadNode neNode = this.CreateRoadNode(this.originX, this.originY, value);
                    this.originY += this.resultMap.nodeHeight;
                }
                this.originX += this.resultMap.nodeWidth;
            }
        }
        public override Vector2 GetClick(float x, float y)
        {
            float cx = Mathf.Floor(x / this.resultMap.nodeWidth);
            float cy = Mathf.Floor(y / this.resultMap.nodeHeight);
            return new Vector2(cx, cy);
        }

        public override void DrawGrid(RoadNode node, LineRenderer renderer)
        {
        }

        public override Mesh[] CreateMapMesh(int minX, int maxX, int minY, int maxY)
        {
            List<MeshData> meshDatas = new List<MeshData>();

            float width_half = this.resultMap.nodeWidth/2;
            float height_half = this.resultMap.nodeHeight/2;


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

                    this.nodeArr.TryGetValue(x+"_"+y, out RoadNode road);
                    if (road == null) return new Mesh[] { };

                    float2 node = new float2(road.cx, road.cy);

                    data.vertices.Add(new Vector3(node.x - width_half, node.y + height_half));
                    data.vertices.Add(new Vector3(node.x + width_half, node.y + height_half));
                    data.vertices.Add(new Vector3(node.x + width_half, node.y - height_half));
                    data.vertices.Add(new Vector3(node.x - width_half, node.y - height_half));

                    int index = data.vertices.Count - 4;

                    data.lines.Add(index);
                    data.lines.Add(index + 1);

                    data.lines.Add(index + 1);
                    data.lines.Add(index + 2);

                    data.lines.Add(index + 2);
                    data.lines.Add(index + 3);

                    data.lines.Add(index + 3);
                    data.lines.Add(index);
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

            Vector3[] vertices = new Vector3[4];
            Vector2[] uv = new Vector2[4];
            int[] triangles = new int[3 * 2];

            vertices[0] = new Vector3(-(width / 2), (height / 2));
            vertices[1] = new Vector3((width / 2), (height / 2));
            vertices[2] = new Vector3(-(width / 2), -(height / 2));
            vertices[3] = new Vector3((width / 2), -(height / 2));

            uv[0] = new Vector2(0, 1);
            uv[1] = new Vector2(1, 1);
            uv[2] = new Vector2(0, 0);
            uv[3] = new Vector2(1, 0);

            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 2;
            triangles[3] = 2;
            triangles[4] = 1;
            triangles[5] = 3;

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;

            return mesh;
        }
    }
}
#endif
