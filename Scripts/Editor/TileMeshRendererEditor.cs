#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace toinfiniityandbeyond.Tilemapping
{
    [CustomEditor(typeof(TileMeshRenderer))]
    public class TileMeshRendererEditor : TileRendererEditor
    {

    }
}
#endif
