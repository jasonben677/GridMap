using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshData 
{
    public List<Vector3> vertices;
    public List<int> lines;

    public MeshData()
    {
        this.vertices = new List<Vector3>();
        this.lines = new List<int>();
    }

}
