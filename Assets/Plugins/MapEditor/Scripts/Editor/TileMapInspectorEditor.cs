#if UNITY_EDITOR
using Cysharp.Threading.Tasks;
using GridMap;
using MapFrame;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using toinfiniityandbeyond.Utillity;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace toinfiniityandbeyond.Tilemapping
{
    partial class TileMapEditor : Editor
    {
        // 輸出名稱
        SerializedProperty spMapName;

        // 節點類型
        SerializedProperty spMapType;

        // 輸出類型
        SerializedProperty spOutputType;
        SerializedProperty spIsFormat;

        // 地圖長寬(不包括邊界)
        SerializedProperty spWidth;
        SerializedProperty spHeight;

        // 節點長寬
        SerializedProperty spGridWidth;
        SerializedProperty spGridHeight;

        // 邊界長寬
        SerializedProperty spCamSizeX;
        SerializedProperty spCamSizeY;

        SerializedProperty spSlice;

        SerializedProperty spWorkAreaID;

        // 選取框圖片
        SerializedProperty rectangleImg;

        // 地圖mesh的根節點
        SerializedProperty meshRoot;

        // 地圖mesh的prefab
        SerializedProperty meshPrefab;

        partial void OnInspectorEnable()
        {
            this.spMapName = serializedObject.FindProperty("mapName");

            this.spMapType = serializedObject.FindProperty("mapType");
            this.spOutputType = serializedObject.FindProperty("outputType");
            this.spIsFormat = serializedObject.FindProperty("isFormat");

            this.spWidth = serializedObject.FindProperty("mapWidth");
            this.spHeight = serializedObject.FindProperty("mapHeight");

            this.spGridWidth = serializedObject.FindProperty("gridWidth");
            this.spGridHeight = serializedObject.FindProperty("gridHeight");

            this.spCamSizeX = serializedObject.FindProperty("marginX");
            this.spCamSizeY = serializedObject.FindProperty("marginY");

            this.spSlice = serializedObject.FindProperty("slice");
            this.spWorkAreaID = serializedObject.FindProperty("workArea");

            this.rectangleImg = serializedObject.FindProperty("rectangleImg");

            this.meshRoot = serializedObject.FindProperty("meshRoot");
            this.meshPrefab = serializedObject.FindProperty("meshPrefab");
        }

        partial void OnInspectorDisable()
        {

        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            tileMap.isInEditMode = GUILayout.Toggle(tileMap.isInEditMode, "", "Button", GUILayout.Height(EditorGUIUtility.singleLineHeight * 1.5f));
            string toggleButtonText = (tileMap.isInEditMode ? "Exit" : "Enter") + " Edit Mode";
            GUI.Label(GUILayoutUtility.GetLastRect(), toggleButtonText, MyStyles.centerBoldLabel);

            if (EditorGUI.EndChangeCheck())
            {
                if (tileMap.isInEditMode)
                    OnEnterEditMode();
                else
                    OnExitEditMode();
            }

            EditorGUILayout.Space();
            string mapName = spMapName.stringValue;
            mapName = EditorGUILayout.TextField("Map Id", mapName);
            if (mapName != spMapName.stringValue)
            {
                tileMap.mapName = mapName;
            }

            #region 節點類型

            EditorGUILayout.Space();
            MapType mapType = (MapType)spMapType.intValue;
            mapType = (MapType)EditorGUILayout.EnumPopup(spMapType.displayName, mapType);
            if ((int)mapType != spMapType.intValue)
            {
                if (EditorUtility.DisplayDialog("警告", "是否要切換Grid類性，資料會被清除", "確定切換", "返回") == false) return;
                this.tileMap.ChangeMapType(mapType);
                this.tileMap.GridSizeChange(this.tileMap.countX, this.tileMap.countY);
                this.tileMap.Reset();
            }

            #endregion

            #region 設定數值(地圖，格子，邊界)

            EditorGUILayout.Space();
            GUILayout.Label("地圖大小 (單位: px)");
            int width = spWidth.intValue;
            width = EditorGUILayout.IntField(spWidth.displayName, width);
            int height = spHeight.intValue;
            height = EditorGUILayout.IntField(spHeight.displayName, height);
            if (width != spWidth.intValue || height != spHeight.intValue)
            {
                this.tileMap.ResizeMapSize(width, height);
                this.tileMap.GridSizeChange(this.tileMap.countX, this.tileMap.countY);
                OnSceneGUI();
            }


            EditorGUILayout.Space();
            GUILayout.Label(new GUIContent("格子大小 (單位: px)"));
            int nodeSizeX = this.spGridWidth.intValue;
            nodeSizeX = EditorGUILayout.IntField(this.spGridWidth.displayName, nodeSizeX);

            if (this.tileMap.mapType == (int)MapType.Hexagonal)
            {
                if (nodeSizeX != this.spGridWidth.intValue)
                {
                    this.tileMap.ResizeNodeSize(nodeSizeX, nodeSizeX);
                }
            }
            else
            {
                int nodeSizeY = this.spGridHeight.intValue;
                nodeSizeY = EditorGUILayout.IntField(this.spGridHeight.displayName, nodeSizeY);

                if (this.tileMap.isInEditMode && (nodeSizeX != this.spGridWidth.intValue || nodeSizeY != this.spGridHeight.intValue))
                {
                    if (EditorUtility.DisplayDialog("警告", "無法在編輯模式中改變網格大小!!!", "確定")) return;
                }
                else if (nodeSizeX != this.spGridWidth.intValue || nodeSizeY != this.spGridHeight.intValue)
                {
                    this.tileMap.ResizeNodeSize(nodeSizeX, nodeSizeY);
                    Debug.Log("<color=#FF7606>【Grid Width】 or 【Grid Height】 is changed. Don't forget Reset !!!</color>");
                }
            }

            EditorGUILayout.Space();
            GUILayout.Label("邊界大小 (單位: px)");
            int camSizeX = this.spCamSizeX.intValue;
            camSizeX = EditorGUILayout.IntField(this.spCamSizeX.displayName, camSizeX);

            int camSizeY = this.spCamSizeY.intValue;
            camSizeY = EditorGUILayout.IntField(this.spCamSizeY.displayName, camSizeY);
            if (camSizeX != this.spCamSizeX.intValue || camSizeY != this.spCamSizeY.intValue)
            {
                this.tileMap.ResizeCameraSize(camSizeX, camSizeY);
            }

            EditorGUILayout.Space();
            GUILayout.Label("工作區數量 (ex: 2*2)");
            int slice = this.spSlice.intValue;
            slice = EditorGUILayout.IntSlider(this.spWorkAreaID.displayName, slice, 1, 16);
            if (slice != this.spSlice.intValue)
            {
                this.tileMap.ChangeSlice(slice);
            }

            if (this.spSlice.intValue > 0)
            {
                EditorGUILayout.Space();
                GUILayout.Label("工作區選擇");
                int max = this.spSlice.intValue * this.spSlice.intValue;
                int count = this.spWorkAreaID.intValue;

                count = EditorGUILayout.IntSlider(this.spWorkAreaID.displayName, count, 1, max);

                if (count != this.spWorkAreaID.intValue)
                {
                    //Debug.LogError(count);
                    this.tileMap.ChangeWorkArea(count);
                }

            }

            #endregion

            #region 元件區

            EditorGUILayout.Space();
            Object rectImg = EditorGUILayout.ObjectField(this.rectangleImg.displayName, this.rectangleImg.objectReferenceValue, typeof(Image), true);
            if (rectImg != this.rectangleImg.objectReferenceValue)
            {
                this.tileMap.ChangeRectangleImage(rectImg);
            }

            EditorGUILayout.Space();
            Object mesh_root = EditorGUILayout.ObjectField(this.meshRoot.displayName, this.meshRoot.objectReferenceValue, typeof(Transform), true);
            if (mesh_root != this.meshRoot.objectReferenceValue)
            {
                this.tileMap.ChangeMeshRoot(mesh_root);
            }

            EditorGUILayout.Space();
            Object mesh_prefab = EditorGUILayout.ObjectField(this.meshPrefab.displayName, this.meshPrefab.objectReferenceValue, typeof(GameObject), true);
            if (mesh_prefab != this.meshPrefab.objectReferenceValue)
            {
                this.tileMap.ChangeMeshPrefab(mesh_prefab);
            }

            #endregion

            #region 刷新/重置地圖

            EditorGUILayout.Space();
            GUILayout.Label("Tools", MyStyles.leftBoldLabel);

            GUILayout.BeginHorizontal();
            GUI.color = new Color(1f, 0.5f, 0.5f);
            if (GUILayout.Button("Reset"))
            {
                if (EditorUtility.DisplayDialog("警告", "你要重置全部節點嗎?", "確定", "返回"))
                {
                    this.ResetEvent();
                }
            }
            GUI.color = Color.white;
            GUILayout.EndHorizontal();

            #endregion

            #region 顯示/隱藏地圖grid狀況
            if (this.tileMap.isInEditMode == false)
            {
                EditorGUILayout.Space();
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Show Preview"))
                {
                    if (EditorUtility.DisplayDialog("警告", "你要顯示全部節點嗎?", "確定", "返回"))
                    {
                        this.tileMap.ShowOrCloseAllGrid(true);
                    }
                }
                if (GUILayout.Button("Close Preview"))
                {
                    if (EditorUtility.DisplayDialog("警告", "你要隱藏全部節點嗎?", "確定", "返回"))
                    {
                        this.tileMap.ShowOrCloseAllGrid(false);
                    }
                }
                GUI.color = Color.white;
                GUILayout.EndHorizontal();
            }
            #endregion

            if (this.tileMap.isInEditMode)
            {
                // 加載資料
                this.LoadOldData();

                #region 輸出檔案

                GUILayout.BeginHorizontal();

                GUIContent[] exportNames = new GUIContent[] { new GUIContent("All"), new GUIContent("json (Client)"), new GUIContent("s (Server)") };
                int[] exportType = { 0, 1, 2 };
                this.tileMap.outputType = EditorGUILayout.IntPopup(new GUIContent("輸出類型", "會依照對應類型自動建立資料夾"), this.tileMap.outputType, exportNames, exportType);
                //ExportType exportType = (ExportType)spOutputType.intValue;
                //exportType = (ExportType)EditorGUILayout.EnumPopup("輸出類型", (ExportType)this.tileMap.outputType);
                //if (exportType != (ExportType)spOutputType.intValue)
                //{
                //    this.tileMap.outputType = (int)exportType;
                //}
                switch ((ExportType)this.tileMap.outputType)
                {
                    case ExportType.All:
                    case ExportType.Client:
                        EditorGUIUtility.labelWidth = 50;
                        this.spIsFormat.boolValue = EditorGUILayout.Toggle(new GUIContent("Format", "Only json type."), this.spIsFormat.boolValue);
                        this.tileMap.isFormat = this.spIsFormat.boolValue;
                        EditorGUIUtility.labelWidth = 20;
                        break;
                }

                if (GUILayout.Button("Export"))
                {
                    if (this.tileMap.isInEditMode == false) return;

                    FileControl.ExtensionFilter[] filter_server = new FileControl.ExtensionFilter[1] { new FileControl.ExtensionFilter("s", "s") };
                    FileControl.ExtensionFilter[] filter_client = new FileControl.ExtensionFilter[1] { new FileControl.ExtensionFilter("json", "json") };
                    FileControl.ExtensionFilter[] filter_all = new FileControl.ExtensionFilter[1] { new FileControl.ExtensionFilter("all", "repl_ext") };

                    string oldPath = PlayerPrefs.GetString("exportPath");
                    string oldName = PlayerPrefs.GetString("exportName");
                    string path = "";

                    switch ((ExportType)this.tileMap.outputType)
                    {
                        case ExportType.All:
                            path = FileControl.FileManager.SaveFilePanel("export", oldPath, oldName, filter_all);
                            this.WriteData_Client(path, true);
                            this.WriteData_Server(path, true);

                            break;

                        case ExportType.Client:
                            path = FileControl.FileManager.SaveFilePanel("export", oldPath, oldName, filter_client);
                            this.WriteData_Client(path, true);
                            break;

                        case ExportType.Server:
                            path = FileControl.FileManager.SaveFilePanel("export", oldPath, oldName, filter_server);
                            this.WriteData_Server(path, true);
                            break;

                        default:
                            break;
                    }

                    if (path.Length > 0)
                    {
                        PlayerPrefs.SetString("exportPath", FileControl.FileManager.GetPathFolder(path));
                        PlayerPrefs.SetString("exportName", Path.GetFileNameWithoutExtension(path));
                    }

                }

                GUILayout.EndHorizontal();

                #endregion
            }

            // 快捷鍵 暫時拿掉
            //if (Event.current.type == EventType.KeyDown && Event.current.control && Event.current.isKey && Event.current.keyCode == KeyCode.Tab)
            //{
            //    tileMap.isInEditMode = !tileMap.isInEditMode;
            //}
            //if (Event.current.type == EventType.KeyDown && Event.current.isKey && Event.current.keyCode == KeyCode.Escape)
            //{
            //    tileMap.isInEditMode = false;
            //}

            serializedObject.ApplyModifiedProperties();
        }

        private async void ResetEvent()
        {
            bool result = await tileMap.GridSizeChange(this.tileMap.countX, this.tileMap.countY);
            tileMap.Reset();
            SetTileMapDirty();
            Debug.LogWarning("重算節點完成");
        }

        private async void LoadOldData()
        {
            GUILayout.BeginHorizontal();
            bool btnClick = GUILayout.Button("Import");
            GUILayout.EndHorizontal();

            if (btnClick)
            {
                if (this.tileMap.isInEditMode == false) return;
                FileControl.ExtensionFilter[] es = new FileControl.ExtensionFilter[1] { new FileControl.ExtensionFilter("json", "json") };

                string oldPath = PlayerPrefs.GetString("savePath");
                string path = FileControl.FileManager.OpenFilePanel("import", oldPath, es, false);

                if (path.Length != 0)
                {
                    PlayerPrefs.SetString("savePath", FileControl.FileManager.GetPathFolder(path));
                    using (StreamReader fs = File.OpenText(path))
                    {
                        int count = MapEditorDefine.renderPerFrame;

                        this.tileMap.mapStruct.OnLoad_ImportMapData(JsonConvert.DeserializeObject<MapData>(fs.ReadToEnd()));
                        this.tileMap.ImportDataSetting();

                        for (int x = 0; x < this.tileMap.countX; x++)
                        {
                            for (int y = 0; y < this.tileMap.countY; y++)
                            {
                                foreach (var tile in this.tileMap.scriptableTileCache)
                                {
                                    if (tile.CheckTile(this.tileMap, x, y))
                                    {
                                        if (count <= 0)
                                        {
                                            await UniTask.Delay(50);
                                            count = MapEditorDefine.renderPerFrame;
                                        }
                                        this.tileMap.SetTileAt(x, y, tile);
                                        count--;
                                    }
                                }
                            }
                        }
                    }
                    Debug.LogWarning("加載舊資料完成");
                }
            }       
        }

        private void WriteData_Server(string path, bool isCreateFolder = false)
        {
            if (path.Length <= 0) return;

            if (path.IndexOf("repl_ext") != -1) path = path.Replace("repl_ext", "s");
            string fileNanme = path.Substring(path.LastIndexOf('\\'));
            string folderName = "\\s";
            path = path.Replace(fileNanme, "");
            string folderPath = $"{path}{folderName}";
            string finalPath;
            if (isCreateFolder)
            {
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
                finalPath = $"{path}{folderName}{fileNanme}";
            }
            else
            {
                finalPath = $"{path}{fileNanme}";
            }

            using (FileStream fs = File.Create(finalPath))
            {
                byte[] info = new UTF8Encoding(true).GetBytes(MapServerUse.OutputMapToServer(this.tileMap.mapStruct));
                fs.Write(info, 0, info.Length);
            }
        }

        private void WriteData_Client(string path, bool isCreateFolder = false)
        {
            if (path.Length <= 0) return;

            if (path.IndexOf("repl_ext") != -1) path = path.Replace("repl_ext", "json");
            string fileNanme = path.Substring(path.LastIndexOf('\\'));
            string folderName = "\\json";
            path = path.Replace(fileNanme, "");
            string folderPath = $"{path}{folderName}";
            string finalPath;
            if (isCreateFolder)
            {
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
                finalPath = $"{path}{folderName}{fileNanme}";
            }
            else
            {
                finalPath = $"{path}{fileNanme}";
            }

            using (FileStream fs = File.Create(finalPath))
            {
                byte[] info = new UTF8Encoding(true).GetBytes(JsonConvert.SerializeObject(this.tileMap.mapStruct.resultMap, (!this.tileMap.isFormat) ? Formatting.None : Formatting.Indented));
                fs.Write(info, 0, info.Length);
            }
        }

        private enum ExportType
        {
            All = 0,
            Client = 1,
            Server = 2
        }

    }
}
#endif