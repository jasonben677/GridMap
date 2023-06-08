#if UNITY_EDITOR
using GridMap;
using UnityEngine;

namespace toinfiniityandbeyond.Tilemapping
{
    [CreateAssetMenu(fileName = "New SimpleTile", menuName = "Tilemap/Tiles/BasicMapType")]
    public class BasicMapType : ScriptableTile
    {
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
            try
            {
                tileMap.mapStruct.resultMap.roadDataArr[x, y] = type;
            }
            catch
            {
                Debug.LogWarning($"({x}, {y}) => Index out of range!!!");
            }

        }
        public override bool CheckTile(TileMap tileMap, int x, int y)
        {
            try
            {
                return (tileMap.mapStruct.resultMap.roadDataArr[x, y] == this.type);
            }
            catch (System.Exception)
            {
                Debug.LogError(string.Format("error : {0},{1}", x,y));
                throw;
            }

           
        }

    }
}
#endif
