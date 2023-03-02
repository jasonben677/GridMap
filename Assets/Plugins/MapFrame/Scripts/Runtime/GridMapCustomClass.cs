using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MapFrame
{
    public class MapData
    {
        public string name = "";
        public string bgName = "";
        public MapType type = MapType.none;
        public float mapWidth = 0;
        public float mapHeight = 0;
        public float nodeWidth = 0;
        public float nodeHeight = 0;
        public float nodeDiagonal = 0;
        public float camWidth = 0;
        public float camHeight = 0;
        public float sizeX = 0;
        public float sizeY = 0;
        public float offsetX = 0;
        public float offsetY = 0;
        public string respawn = "";
        public Dictionary<string, NpcClass> npcs;
        public Dictionary<string, MobArea> mobAreas;
        public Dictionary<string, TeleportStruct> teleport;
        public int[,] roadDataArr;

        public MapData()
        {
            this.npcs = new Dictionary<string, NpcClass>();
            this.mobAreas = new Dictionary<string, MobArea>();
            this.teleport = new Dictionary<string, TeleportStruct>();
        }

        public MapData ShallowCopy()
        {
            return (MapData)this.MemberwiseClone();
        }
    }

    /// <summary>
    /// 傳送點
    /// </summary>
    public class TeleportStruct
    {
        public string map = "";
        public string respawn = "";

        public TeleportStruct(string _map = "point", string _respawn = "point")
        {
            this.map = _map;
            this.respawn = _respawn;
        }
    }

    /// <summary>
    /// 互動選項
    /// </summary>
    public class InteractFuncSelect
    {
        public int order = 0;
        public string name = "";
        public UnityAction func = null;

        public InteractFuncSelect()
        { 
        
        }
        public InteractFuncSelect(string _name, UnityAction _func, int _order = 1)
        {
            this.order = _order;
            this.name = _name;
            this.func = _func;
        }
    }

    /// <summary>
    /// npc
    /// </summary>
    public class NpcClass
    {
        public Dictionary<string, int> faceDirection = new Dictionary<string, int>();
        public List<string> pos = new List<string>();
        public NpcClass()
        { 
        
        }
    }

    /// <summary>
    /// 怪物區域
    /// </summary>
    public class MobArea
    {
        public string color;
        public string areaId;
        public int rate;
        public MobArea()
        {
        }
        public MobArea(string _color, string _areaid, int _rate)
        {
            this.color = _color;
            this.areaId = _areaid;
            this.rate = _rate;
        }
    }

    public enum MapType
    {
        none = -1,
        Angle45 = 0,
        Angle90 = 1,
        Hexagonal = 2,
    }
}
