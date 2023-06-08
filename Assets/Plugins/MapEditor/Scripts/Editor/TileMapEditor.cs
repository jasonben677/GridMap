#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

// For obtaining list of sorting layers.

namespace toinfiniityandbeyond.Tilemapping
{
    [CustomEditor (typeof (TileMap)), CanEditMultipleObjects]
	partial class TileMapEditor : Editor
	{
		[MenuItem("GameObject/2D Object/TileMap")]
		private static void CreateTileMapGameObject()
		{
			GameObject tileObject = new GameObject ("New TileMap", typeof(TileMap));
		}

		private TileMap tileMap;

		partial void OnInspectorEnable ();
		partial void OnInspectorDisable ();
		partial void OnSceneEnable ();
		partial void OnSceneDisable ();
		
		private void OnEnable ()
		{
			tileMap = (TileMap)target;
			OnInspectorEnable ();
			OnSceneEnable ();
		}

		private void OnDisable ()
		{
			OnInspectorDisable ();
			OnSceneDisable ();
		}
	}
}
#endif