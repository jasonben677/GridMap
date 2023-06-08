using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
namespace toinfiniityandbeyond.Tilemapping
{
	[Serializable]
	public class Eraser : Brush
	{
		public Eraser () : base ()
		{
			radius = 1;
		}
		public override KeyCode Shortcut { get { return KeyCode.E; } }
		public override string Description { get { return "Sets the painted tile to nothing"; } }

		public override void OnClick (Point point, ScriptableTile tile, TileMap map)
		{
			base.OnClick(point, null, map);
		}

        public override void OnClickUp(Point point, ScriptableTile tile, TileMap map)
        {
            base.OnClickUp(point, null, map);
        }
    }
}
#endif
