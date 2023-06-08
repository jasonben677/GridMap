#if UNITY_EDITOR
using Cysharp.Threading.Tasks;
using GridMap;
using MapFrame;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace toinfiniityandbeyond.Tilemapping
{
    [ExecuteInEditMode, AddComponentMenu("2D/TileMap"), DisallowMultipleComponent]
    public class TileMap : MonoBehaviour
    {
        [SerializeField]
        public string mapName = "";

        [SerializeField]
        public int mapType = 0;

        [SerializeField]
        public int outputType = 0;
        [SerializeField]
        public bool isFormat = false;

        [SerializeField]
        public int mapWidth = 1024, mapHeight = 1024;

        [SerializeField]
        public int gridWidth = 32, gridHeight = 32;

        [SerializeField]
        public int marginX = 128, marginY = 256;

        [SerializeField]
        public int slice = 1;

        [SerializeField]
        public int workArea = 1;

        [SerializeField]
        public Image rectangleImg;

        [SerializeField]
        public Transform meshRoot;

        [SerializeField]
        public GameObject meshPrefab;

        // 每個格子上的資訊
        [SerializeField]
        public ScriptableTile[] map = new ScriptableTile[0];

        // 寬的格子數(包括邊界)
        public int countX { get { return Mathf.FloorToInt((this.mapWidth + this.marginX * 2) / this.gridWidth); } }
        // 高的格子數(包括邊界)
        public int countY { get { return Mathf.FloorToInt(((this.mapHeight + this.marginY * 2) / this.gridHeight) * this.specialFormat); } }

        // 工作區的X範圍
        public (int min, int max) xRange;
        // 工作區的Y範圍
        public (int min, int max) yRange;

        // 45度和蜂巢狀高度需要特別算
        public float specialFormat = 1;

        public IMapBase mapStruct;

        public Action<int, int> OnUpdateTileAt = (x, y) => { };
        public Action OnUpdateTileMap = () => { };
        public Action<int, int> OnResize = (width, height) => { };

        private bool CurrentOperation = false;
        private List<ChangeElement> CurrentEdit;
        private Timeline timeline;


        private void Update()
        {
            //for (int x = 0; x < countX; x++)
            //{
            //    for (int y = 0; y < countY; y++)
            //    {
            //        ScriptableTile tile = GetTileAt(x, y);
            //        //if (tile && tile.CheckIfCanTick())
            //            UpdateTileAt(x, y);
            //    }
            //}
            //await UniTask.Delay(1000);
        }

        public void Reset()
        {
            map = new ScriptableTile[countX * countY];
            timeline = new Timeline();
            CurrentEdit = new List<ChangeElement>();

            this.ChangeWorkArea(this.workArea);

            this.SetMapStruct();
            this.SetMapdata();
            this.mapStruct.OnLoad_NewMapdata();
            this.mapStruct.CreateGrid(true);

            UpdateTileMap();

        }

        public void ImportDataSetting()
        {
            MapData md = this.mapStruct.resultMap.ShallowCopy();

            this.mapName = this.mapStruct.resultMap.name;

            this.ChangeMapType(this.mapStruct.resultMap.type);

            this.mapWidth = Mathf.RoundToInt(this.mapStruct.resultMap.mapWidth * MapEditorDefine.scale);
            this.mapHeight = Mathf.RoundToInt(this.mapStruct.resultMap.mapHeight * MapEditorDefine.scale);

            this.gridWidth = Mathf.RoundToInt(this.mapStruct.resultMap.nodeWidth * MapEditorDefine.scale);
            this.gridHeight = Mathf.RoundToInt(this.mapStruct.resultMap.nodeHeight * MapEditorDefine.scale);

            this.marginX = Mathf.RoundToInt(this.mapStruct.resultMap.camWidth * MapEditorDefine.scale * 0.5f);
            this.marginY = Mathf.RoundToInt(this.mapStruct.resultMap.camHeight * MapEditorDefine.scale * 0.5f);

            this.Reset();
            this.GridSizeChange(this.countX, this.countY).Forget();

            this.mapStruct.resultMap = md;
            UpdateTileMap();

        }

        /// <summary>
        /// 重算節點數量
        /// </summary>
        /// <param name="newWidth">寬的格子數</param>
        /// <param name="newHeight">高的格子數</param>
        public async UniTask<bool> GridSizeChange(int newWidth, int newHeight)
        {
            Debug.LogWarning("重算節點");
            int oldWidth = countX, oldHeight = countY;
            ScriptableTile[] oldMap = map;
            map = new ScriptableTile[newWidth * newHeight];
            OnResize.Invoke(newWidth, newHeight);

            int count = MapEditorDefine.renderPerFrame;

            for (int i = 0; i < oldMap.Length; i++)
            {
                if (count <= 0)
                {
                    await UniTask.Delay(50);
                    count = 3000;
                }

                int x = i % oldWidth;
                int y = i / oldWidth;

                ScriptableTile tile = oldMap[i];
                if (tile && IsInBounds(x, y)) SetTileAt(x, y, tile);
                count--;
            }

            Debug.LogWarning("節點大小調整完成");
            return true;
        }

        /// <summary>
        /// 修改地圖節點類型
        /// </summary>
        /// <param name="mapType"></param>
        public void ChangeMapType(MapType mapType)
        {
            this.mapType = (int)mapType;
            switch (mapType)
            {
                case MapType.none:
                case MapType.Angle90:
                    this.specialFormat = 1;
                    break;

                case MapType.Angle45:
                    this.specialFormat = 2;
                    break;

                case MapType.Hexagonal:
                    this.specialFormat = (100 / 75f);
                    this.gridHeight = this.gridWidth;
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// 修改地圖長寬(不含邊界)
        /// </summary>
        public void ResizeMapSize(int newWidth, int newHeight)
        {
            this.mapWidth = newWidth;
            this.mapHeight = newHeight;
        }

        /// <summary>
        /// 修改格子長寬
        /// </summary>
        public void ResizeNodeSize(int newWidth, int newHeight)
        {
            this.gridWidth = newWidth;
            this.gridHeight = newHeight;
            this.GridSizeChange(this.countX, this.countY).Forget();
        }

        /// <summary>
        /// 修改攝影機長寬(邊界)
        /// </summary>
        public void ResizeCameraSize(int newWidth, int newHeight)
        {
            this.marginX = newWidth;
            this.marginY = newHeight;

            this.GridSizeChange(this.countX, this.countY).Forget();
        }

        /// <summary>
        /// 切工作區數量
        /// </summary>
        /// <param name="count">數量(會是 count*count個)</param>
        public void ChangeSlice(int count)
        {
            this.slice = count;
        }

        /// <summary>
        /// 選擇工作區
        /// </summary>
        /// <param name="id">工作區id</param>
        public void ChangeWorkArea(int id)
        {

            this.workArea = id;

            var oldXrange = (this.xRange.min, this.xRange.max);
            var oldYrange = (this.yRange.min, this.yRange.max);

            int xSize = this.countX / this.slice;
            int ySize = this.countY / this.slice;

            int xLevel = (id - 1) % this.slice;
            int yLevel = (id - 1) / this.slice;

            this.xRange = (xSize * xLevel, (xSize * xLevel) + (xSize));
            this.yRange = (ySize * yLevel, (ySize * yLevel) + (ySize));


            //Debug.LogError("sizex: " + xSize + "old x: " + oldXrange + "sizeY" + ySize + "old y :" + oldYrange);
            //Debug.LogError("new x: " + this.xRange + "new y :" + this.yRange);


            this.SetMapMesh(this.xRange.min, this.xRange.max, this.yRange.min, this.yRange.max);

            // 切換工作區
            TileMeshRenderer tm = this.gameObject.GetComponent<TileMeshRenderer>();
         
            for (int x = oldXrange.min; x < oldXrange.max; x++)
            {
                for (int y = oldYrange.min; y < oldYrange.max; y++)
                {
                    tm.ShowOrCloseTile(x, y, false);
                }
            }

            for (int x = this.xRange.min; x < this.xRange.max; x++)
            {
                for (int y = yRange.min; y < yRange.max; y++)
                {
                    tm.ShowOrCloseTile(x, y, true);
                }
            }

        }

        /// <summary>
        /// 修改選取框
        /// </summary>
        public void ChangeRectangleImage(UnityEngine.Object img)
        {
            this.rectangleImg = img as Image;
        }

        public void ChangeMeshRoot(UnityEngine.Object trans)
        {
            this.meshRoot = trans as Transform;
        }

        public void ChangeMeshPrefab(UnityEngine.Object go)
        {
            this.meshPrefab = go as GameObject;
        }

        /// <summary>
        /// 設定節點類型0 : 45度, 1: 90度, 2: 蜂巢狀
        /// </summary>
        public void SetMapStruct()
        {
            switch (this.mapType)
            {
                case 0:
                    this.mapStruct = new RuntimeMap45();
                    break;

                case 1:
                    this.mapStruct = new RuntimeMap90();
                    break;

                case 2:
                    this.mapStruct = new RuntimeMapHexagonal();
                    break;

                default:
                    this.mapStruct = new RuntimeMap90();
                    break;
            }
        }

        public void SetMapdata()
        {
            this.mapStruct.resultMap.name = this.mapName;
            this.mapStruct.resultMap.bgName = this.mapName;
            this.mapStruct.resultMap.type = (MapType)this.mapType;

            this.mapStruct.resultMap.mapWidth = this.mapWidth;
            this.mapStruct.resultMap.mapHeight = this.mapHeight;

            this.mapStruct.resultMap.nodeWidth = this.gridWidth;
            this.mapStruct.resultMap.nodeHeight = this.gridHeight;
            this.mapStruct.CalculateDiagonal();

            this.mapStruct.resultMap.camWidth = this.marginX * 2;
            this.mapStruct.resultMap.camHeight = this.marginY * 2;

            this.mapStruct.resultMap.sizeX = this.countX;
            this.mapStruct.resultMap.sizeY = this.countY;

            this.mapStruct.ChangeUnitToM();

        }

        public void SetMapMesh(int minX, int maxX, int minY, int maxY)
        {
            //Debug.LogError("set map");
            if (this.mapStruct != null)
            {
                int count = this.meshRoot.childCount;

                for (int i = 0; i < count; i++)
                {
                    GameObject.DestroyImmediate(this.meshRoot.GetChild(0).gameObject);
                }


                Mesh[] meshs = this.mapStruct.CreateMapMesh(minX, maxX, minY, maxY);

                for (int i = 0; i < meshs.Length; i++)
                {
                    GameObject m = Instantiate(this.meshPrefab, this.meshRoot);

                    m.GetComponent<MeshFilter>().mesh = meshs[i];
                }
            }
        }

        /// <summary>
        /// 是否在地圖範圍內(含邊界)
        /// </summary>
        /// <returns></returns>
        public bool IsInBounds(int x, int y)
        {
            return (x >= 0 && x < countX && y >= 0 && y < countY);
        }


        public bool IsInWorkArea(int x, int y)
        {
            if (x < this.xRange.min || x >= this.xRange.max) return false;
            if (y < this.yRange.min || y >= this.yRange.max) return false;

            return true;
        }

        /// <summary>
        /// 顯示或隱藏所有grid
        /// </summary>
        /// <param name="isShow"></param>
        public void ShowOrCloseAllGrid(bool isShow = false)
        {
            // 切換工作區
            TileMeshRenderer tm = this.gameObject.GetComponent<TileMeshRenderer>();

            for (int x = 0; x < this.countX; x++)
            {
                for (int y = 0; y < this.countY; y++)
                {
                    tm.ShowOrCloseTile(x, y, isShow);
                }
            }

        }

        /// <summary>
        /// 取得格子資訊
        /// </summary>
        /// <returns></returns>
        public ScriptableTile GetTileAt(int x, int y)
        {
            if (!IsInBounds(x, y))
                return null;

            int index = x + y * countX;

            if (index > map.Length) return null;

            try
            {
                return map[index];
            }
            catch
            {
                Debug.LogWarning($"({x}, {y}). Index: {index}, Length: {map.Length}.");
                return null;
            }
        }

        /// <summary>
        /// 修改格子資訊
        /// </summary>
        /// <returns></returns>
        public bool SetTileAt(int x, int y, ScriptableTile to, bool canReplaceMobArea = false)
        {
            ScriptableTile from = GetTileAt(x, y);
            //Conditions for returning
            if (IsInBounds(x, y) && !(from == null && to == null) && (((from == null || to == null) && (from != null || to != null)) || from.ID != to.ID))
            {
                if (from != null)
                {
                    if (from.type == (int)NodeType.Mob && canReplaceMobArea == false) return false;
                    from.RemoveFromMapData(this, x, y);
                }
                if (to != null) to.WriteToMapData(this, x, y);

                map[x + y * countX] = to;

                if (CurrentEdit == null)
                    CurrentEdit = new List<ChangeElement>();
                CurrentEdit.Add(new ChangeElement(x, y, from, to));
                UpdateTileAt(x, y);
                UpdateNeighbours(x, y, true);

                return true;
            }
            return false;
        }
        public void UpdateTileAt(int x, int y)
        {
            OnUpdateTileAt.Invoke(x, y);
        }

        public void UpdateNeighbours(int x, int y, bool incudeCorners = false)
        {
            for (int xx = -1; xx <= 1; xx++)
            {
                for (int yy = -1; yy <= 1; yy++)
                {
                    if (xx == 0 && yy == 0)
                        continue;

                    if (!incudeCorners && !(xx == 0 || yy == 0))
                        continue;

                    if (IsInBounds(x + xx, y + yy))
                        UpdateTileAt(x + xx, y + yy);
                }
            }
        }
        public void UpdateType(ScriptableTile type)
        {
            for (int x = 0; x <= this.countX; x++)
            {
                for (int y = 0; y <= this.countY; y++)
                {
                    if (GetTileAt(x, y) == type)
                    {
                        UpdateTileAt(x, y);
                    }
                }
            }
        }
        public List<ScriptableTile> GetAllTileTypes()
        {
            List<ScriptableTile> result = new List<ScriptableTile>();
            for (int x = 0; x <= this.countX; x++)
            {
                for (int y = 0; y <= this.countY; y++)
                {
                    ScriptableTile tile = GetTileAt(x, y);
                    if (!result.Contains(tile) && tile != null)
                    {
                        result.Add(tile);
                    }
                }
            }
            return result;
        }

        public void UpdateTileMap()
        {
            OnUpdateTileMap.Invoke();
        }

        public bool CanUndo
        {
            get { return (timeline != null && timeline.CanUndo); }
        }
        public bool CanRedo
        {
            get { return (timeline != null && timeline.CanRedo); }
        }

        public void Undo()
        {
            if (timeline == null)
                return;
            List<ChangeElement> changesToRevert = timeline.Undo();

            foreach (var c in changesToRevert)
            {
                map[c.x + c.y * countX] = c.from;
                UpdateTileAt(c.x, c.y);
                UpdateNeighbours(c.x, c.y, true);
            }
        }

        public void Redo()
        {
            if (timeline == null)
                return;
            List<ChangeElement> changesToRevert = timeline.Redo();

            foreach (var c in changesToRevert)
            {
                map[c.x + c.y * countX] = c.to;
                UpdateTileAt(c.x, c.y);
                UpdateNeighbours(c.x, c.y, true);
            }
        }

        public void BeginOperation()
        {
            CurrentOperation = true;
            CurrentEdit = new List<ChangeElement>();
        }

        public void FinishOperation()
        {
            CurrentOperation = false;
            if (timeline == null)
                timeline = new Timeline();
            timeline.PushChanges(CurrentEdit);
        }

        public bool OperationInProgress()
        {
            return CurrentOperation;
        }

        public bool HasEditRecord()
        {
            if (this.CurrentEdit == null) return false;

            return this.CurrentEdit.Count > 0;
        }

        //A cheat-y way of serialising editor variables in the Unity Editor

        public bool isInEditMode = false;

        public ScriptableTile primaryTile, secondaryTile;

        public Rect toolbarWindowPosition, tilePickerWindowPosition;
        public Vector2 tilePickerScrollView;

        public int selectedScriptableTool = -1, lastSelectedScriptableTool = -1;

        public bool primaryTilePickerToggle = false, secondaryTilePickerToggle = false;

        public List<ScriptableTool> scriptableToolCache = new List<ScriptableTool>();
        public List<ScriptableTile> scriptableTileCache = new List<ScriptableTile>();

        public Vector3 tileMapPosition;
        public Quaternion tileMapRotation;

    }
}
#endif

