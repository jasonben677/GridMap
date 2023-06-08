#if UNITY_EDITOR
using GridMap;
using MapFrame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace toinfiniityandbeyond.Tilemapping
{
    [CreateAssetMenu(fileName = "New SimpleTile", menuName = "Tilemap/Tiles/TeleportType")]
    public class TeleportType : ScriptableTile
    {
        public string to_MapName;
        public Vector2 to_MapPos;

        public Material mat;
        public NodeType nodeType;

        public override bool IsValid
        {
            get
            {
                return true;
            }
        }

        public override int type
        {
            get
            {
                return (int)nodeType;
            }
        }

        public override Texture2D GetIcon()
        {
            return mat.mainTexture as Texture2D;
        }

        public override Material GetMaterial()
        {
            return mat;
        }
        public override void RemoveFromMapData(TileMap tileMap, int x, int y)
        {
            tileMap.mapStruct.resultMap.roadDataArr[x, y] = (int)NodeType.Block;
            if (tileMap.mapStruct.resultMap.teleport.ContainsKey(x + "_" + y))
            {
                tileMap.mapStruct.resultMap.teleport.Remove(x + "_" + y);
            }
        }

        public override void WriteToMapData(TileMap tileMap, int x, int y)
        {
            TeleportStruct teleport = new TeleportStruct(this.to_MapName, string.Format("{0}_{1}", this.to_MapPos.x, this.to_MapPos.y));
            tileMap.mapStruct.resultMap.roadDataArr[x, y] = type;

            tileMap.mapStruct.resultMap.teleport[x + "_" + y] = teleport;
        }
        public override bool CheckTile(TileMap tileMap, int x, int y)
        {
            if (tileMap.mapStruct.resultMap.roadDataArr[x, y] != type) return false;

            foreach (var teleport in tileMap.mapStruct.resultMap.teleport)
            {
                bool result = (teleport.Key == x + "_" + y) && (teleport.Value.map == this.to_MapName) && (teleport.Value.respawn == string.Format("{0}_{1}", this.to_MapPos.x, this.to_MapPos.y));
                if (result == true) return true;
            }

            return false;
        }
    }
}
#endif
