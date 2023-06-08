#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;
using GridMap;
using MapFrame;
using Cysharp.Threading.Tasks;

namespace toinfiniityandbeyond.Tilemapping
{
    [Serializable]
    public class Rectangle : ScriptableTool
    {
        public bool filled;

        private Point start;
        private Point end;

        private int minX;
        private int maxX;
        private int minY;
        private int maxY;

        //An empty constructor
        public Rectangle() : base()
        {
            filled = true;
        }
        //Sets the shortcut key to 'P'
        public override KeyCode Shortcut
        {
            get { return KeyCode.W; }
        }
        //Sets the tooltip description
        public override string Description
        {
            get { return "Draws a rectangle"; }
        }
        //Called when the left mouse button is held down
        public override void OnClick(Point point, ScriptableTile tile, TileMap map)
        {
            //Return if the tilemap is null/empty
            if (map == null)
                return;

            //If we haven't already started an operation, start one now
            //This is for undo/ redo support
            if (!map.OperationInProgress())
                map.BeginOperation();

            end = point;
            //Set the tile at the specified point to the specified tile

            map.mapStruct.nodeArr.TryGetValue(point.x + "_" + point.y, out RoadNode roadNode);
            if (roadNode != null)
            {
                float scaleX = (roadNode.cx - map.rectangleImg.transform.position.x) / (map.gridWidth / MapEditorDefine.scale);
                float scaleY = (roadNode.cy - map.rectangleImg.transform.position.y) / (map.gridHeight / MapEditorDefine.scale);
                map.rectangleImg.transform.localScale = new Vector3(scaleX, -scaleY, 1);
            }

        }
        //Called when the left mouse button is initially held down
        public override void OnClickDown(Point point, ScriptableTile tile, TileMap map)
        {
            base.OnClickDown(point, tile, map);
            start = end = point;

            map.mapStruct.nodeArr.TryGetValue(point.x + "_" + point.y, out RoadNode roadNode);
            if (roadNode != null)
            {
                map.rectangleImg.GetComponent<RectTransform>().sizeDelta = new Vector2(map.gridWidth, map.gridHeight);
                map.rectangleImg.transform.position = new Vector2(roadNode.cx, roadNode.cy);
                map.rectangleImg.gameObject.SetActive(true);
            }

        }
        public override void OnClickUp(Point point, ScriptableTile tile, TileMap map)
        {
            bool replace = this._IsInMobArea(tile, map);
            for (int i = 0; i < region.Count; i++)
            {
                map.SetTileAt(region[i].x, region[i].y, tile, replace);
            }
            base.OnClickUp(point, tile, map);

            start = end = point;
            region = new List<Point>();

            map.rectangleImg.gameObject.SetActive(false);
            map.rectangleImg.GetComponent<RectTransform>().sizeDelta = new Vector2(map.gridWidth, map.gridWidth);
            map.rectangleImg.transform.localScale = Vector3.one;
        }
        public override List<Point> GetRegion(Point point, ScriptableTile tile, TileMap map)
        {
            region = new List<Point>();
            if (end == start)
                return base.GetRegion(point, tile, map);

            this.minX = Mathf.Min(start.x, end.x);
            this.maxX = Mathf.Max(start.x, end.x);
            this.minY = Mathf.Min(start.y, end.y);
            this.maxY = Mathf.Max(start.y, end.y);

            for (int x = this.minX; x <= this.maxX; x++)
            {
                for (int y = this.minY; y <= this.maxY; y++)
                {
                    if (filled || (x == this.minX || x == this.maxX || y == this.minY || y == this.maxY))
                    {
                        if (map.IsInWorkArea(x, y))
                        {
                            region.Add(new Point(x, y));
                        }
                    }
                        
                }
            }

            return region;
        }

        private bool _IsInMobArea(ScriptableTile tile, TileMap map)
        {
            string curkey = this.minX + " " + this.minY + " " + this.maxX + " " + this.maxY;
            if (map.mapStruct.resultMap.mobAreas.ContainsKey(curkey))
            {
                map.mapStruct.resultMap.mobAreas.Remove(curkey);
                return true;
            }

            MobAreaType mobs = tile as MobAreaType;
            if (mobs != null)
            {
                mobs.MobAreaTypeWrite(map, new int[] { this.minX, this.maxX }, new int[] { this.minY, this.maxY });
                return true;
            }
            return false;
        }
    }
}
#endif