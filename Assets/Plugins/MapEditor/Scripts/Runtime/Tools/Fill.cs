#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using GridMap;
using MapFrame;
using Cysharp.Threading.Tasks;

namespace toinfiniityandbeyond.Tilemapping
{
	
	[System.Serializable]
	public class Fill : ScriptableTool
	{
		//An empty constructor
		public Fill () : base()
		{

		}
		//Sets the shortcut key to 'F'
		public override KeyCode Shortcut
		{
			get { return KeyCode.F; }
		}
		//Sets the tooltip description
		public override string Description
		{
			get { return "A flood fill tool"; }
		}
		//Called when the left mouse button is held down
		public override void OnClick (Point point, ScriptableTile tile, TileMap map)
		{
		}
		//Called when the left mouse button is initially held down
		public override void OnClickDown (Point point, ScriptableTile tile, TileMap map)
		{
            //Return if the tilemap is null/empty
            if (map == null)
                return;

            base.OnClickDown(point, tile, map);

            region = new List<Point>();

            for (int x = map.xRange.min; x < map.xRange.max; x++)
            {
                for (int y = map.yRange.min; y < map.yRange.max; y++)
                {
                    map.mapStruct.nodeArr.TryGetValue(x + "_" + y, out RoadNode node);
                    if (node != null) region.Add(new Point((int)node.dx, (int)node.dy));
                }
            }

            this._NormalFill(tile, map);
        }
		public override List<Point> GetRegion (Point point, ScriptableTile tile, TileMap map) 
		{
            return null;
        }

		private void _NormalFill(ScriptableTile tile, TileMap map)
		{
			for (int i = 0; i < region.Count; i++)
			{
				Point offsetPoint = region[i];
				map.SetTileAt(offsetPoint.x, offsetPoint.y, tile);
			}

			region = new List<Point>();
		}

	}
}
#endif