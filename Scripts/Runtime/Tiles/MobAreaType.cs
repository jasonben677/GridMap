#if UNITY_EDITOR
using GridMap;
using MapFrame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace toinfiniityandbeyond.Tilemapping
{
    [CreateAssetMenu(fileName = "New SimpleTile", menuName = "Tilemap/Tiles/MobAreaType")]
    public class MobAreaType : ScriptableTile
    {
        public string mobAreaId;
        public float counterRate;
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
        }

        public override void WriteToMapData(TileMap tileMap, int x, int y)
        {
            tileMap.mapStruct.resultMap.roadDataArr[x, y] = type;
        }

        public override bool CheckTile(TileMap tileMap, int x, int y)
        {
            if (tileMap.mapStruct.resultMap.roadDataArr[x, y] != this.type) return false;

            foreach (var mobarea in tileMap.mapStruct.resultMap.mobAreas)
            {
                string[] poses = mobarea.Key.Split(' ');
                if (poses.Length < 4) return false;

                int.TryParse(poses[0], out int x0);
                int.TryParse(poses[1], out int y0);
                int.TryParse(poses[2], out int x1);
                int.TryParse(poses[3], out int y1);

                bool checkX = (x >= x0 && x <= x1);
                bool checkY = (y >= y0 && y <= y1);
                if (checkX && checkY) return true;
            }
            return false;
        }

        public void MobAreaTypeWrite(TileMap tileMap, int[] pairX, int[] pairY)
        {
            MobArea mobArea = new MobArea();
            mobArea.areaId = this.mobAreaId;
            mobArea.rate = Mathf.RoundToInt(this.counterRate);
            tileMap.mapStruct.resultMap.mobAreas[pairX[0] + " " + pairY[0] + " " + pairX[1] + " " + pairY[1]] = mobArea;
        }
    }
}
#endif
