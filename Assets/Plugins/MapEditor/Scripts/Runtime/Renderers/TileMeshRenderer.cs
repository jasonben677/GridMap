#if UNITY_EDITOR

using GridMap;
using MapFrame;
using UnityEngine;
namespace toinfiniityandbeyond.Tilemapping
{
    [AddComponentMenu("2D/Renderer/TileMeshRenderer")]
    public class TileMeshRenderer : TileRenderer
    {
        [SerializeField]
        private MeshRenderer[] meshMap  = new MeshRenderer[0];

        [SerializeField]
        private MeshFilter[] meshFilters = new MeshFilter[0];

        public override void Resize(int width, int height)
        {
            if (width * height == this.meshMap.Length && width * height == this.meshFilters.Length)
                return;

            ClearChildren();

            this.meshMap = new MeshRenderer[width * height];
            this.meshFilters = new MeshFilter[width * height];
        }

        public override void UpdateTileAt(int x, int y)
        {
            int index = x + y * this.tileMap.countX;

            if (index >= this.meshMap.Length)
            {
                Debug.LogError(" x : " + x + "y : " + y + "index : " + index);
                Debug.LogError(" count : " + this.meshMap.Length);
                return;
            }


            MeshRenderer current = this.meshMap[index];
            MeshFilter currentFilter = this.meshFilters[index];

            if (current == null)
            {
                GameObject inst = Instantiate(this.tileMap.meshPrefab, parent);
                current = inst.GetComponent<MeshRenderer>();
                this.meshMap[index] = current;
                inst.name = string.Format("[{0}, {1}]", x, y);
            }

            if(currentFilter == null)
            {
                currentFilter = current.gameObject.GetComponent<MeshFilter>();
                this.meshFilters[index] = currentFilter;
            }

            ScriptableTile tile = tileMap.GetTileAt(x, y);
            current.sharedMaterial = (tile == null) ? material : tile.GetMaterial();
            if (this.tileMap.mapStruct != null) currentFilter.mesh = this.tileMap.mapStruct.DrawMesh();

            if (this.tileMap.mapStruct != null)
            {
                this.tileMap.mapStruct.nodeArr.TryGetValue(x + "_" + y, out RoadNode roadNode);

                if (tile != null && roadNode != null)
                {
                    current.transform.position = new Vector2(roadNode.cx, roadNode.cy);
                }
            }

            current.transform.localScale = Vector2.one;

            // 目前無效
            current.sortingLayerID = sortingLayer;
            current.sortingOrder = orderInLayer;
            //

            current.gameObject.SetActive(this.tileMap.IsInWorkArea(x,y) && tile != null);
        }

        public void ShowOrCloseTile(int x, int y, bool active = false)
        {
            int index = x + y * this.tileMap.countX;

            if (index >= this.meshMap.Length) return;

            MeshRenderer current = this.meshMap[index];
            ScriptableTile tile = tileMap.GetTileAt(x, y);

            current?.gameObject.SetActive(active && tile!= null);
        }

    }
}
#endif