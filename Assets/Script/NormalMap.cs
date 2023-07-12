using MapFrame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalMap : MapBasic
{
    public NormalMap(): base()
    {
        this.astarTypeUse = new AstarFindRoad90();
    }

}
