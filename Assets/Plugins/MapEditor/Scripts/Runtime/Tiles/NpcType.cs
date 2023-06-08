#if UNITY_EDITOR
using GridMap;
using MapFrame;
using UnityEngine;

namespace toinfiniityandbeyond.Tilemapping
{
    [CreateAssetMenu(fileName = "New SimpleTile", menuName = "Tilemap/Tiles/NpcType")]
    public class NpcType : ScriptableTile
    {
        public string npcId;
        public Material mat;
        public FaceDirection faceDirection;
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

            if (tileMap.mapStruct.resultMap.npcs.ContainsKey(this.npcId))
            {
                if (tileMap.mapStruct.resultMap.npcs[this.npcId].pos.Contains(x + "_" + y) == true)
                {
                    tileMap.mapStruct.resultMap.npcs[this.npcId].pos.Remove(x + "_" + y);
                }
                if (tileMap.mapStruct.resultMap.npcs[this.npcId].faceDirection.ContainsKey(x + "_" + y))
                {
                    tileMap.mapStruct.resultMap.npcs[this.npcId].faceDirection.Remove(x + "_" + y);
                }
                if (tileMap.mapStruct.resultMap.npcs[this.npcId].pos.Count <= 0) tileMap.mapStruct.resultMap.npcs.Remove(this.npcId);
            }
        }

        public override void WriteToMapData(TileMap tileMap, int x, int y)
        {
            tileMap.mapStruct.resultMap.roadDataArr[x, y] = type;

            if (tileMap.mapStruct.resultMap.npcs.ContainsKey(npcId))
            {
                if (tileMap.mapStruct.resultMap.npcs[npcId].pos.Contains(x + "_" + y) == false)
                {
                    tileMap.mapStruct.resultMap.npcs[npcId].pos.Add(x + "_" + y);
                    tileMap.mapStruct.resultMap.npcs[npcId].faceDirection[x + "_" + y] = (int)this.faceDirection;
                }
            }
            else
            {
                NpcClass npc = new NpcClass();
                npc.pos.Add(x + "_" + y);
                npc.faceDirection[x + "_" + y] = (int)this.faceDirection;
                tileMap.mapStruct.resultMap.npcs.Add(npcId, npc);
            }
        }
        public override bool CheckTile(TileMap tileMap, int x, int y)
        {
            if (tileMap.mapStruct.resultMap.roadDataArr[x, y] != type) return false;


            foreach (var npc in tileMap.mapStruct.resultMap.npcs)
            {
                bool sameKey = (npc.Key == this.npcId);
                bool InPosList = npc.Value.pos.Contains(x + "_" + y);
                bool sameDirection = npc.Value.faceDirection.TryGetValue(x + "_" + y, out int faceDir);

                if (sameKey && InPosList && sameDirection && faceDir == (int)this.faceDirection) return true;
            }

            return false;
        }
    }
}
#endif
