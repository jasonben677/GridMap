#if UNITY_EDITOR
using UnityEngine;

namespace toinfiniityandbeyond.Tilemapping {
	public class TileMapContainer : ScriptableObject {
		public int width, height;
		public ScriptableTile[] map;
	}
}
#endif
