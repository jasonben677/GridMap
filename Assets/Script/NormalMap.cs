using MapFrame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalMap : MapBasic
{
    public NormalMap(): base()
    {
    
    }

    public void SetMapType(PathFindingMapType pathFindingMapType)
    {
        switch (pathFindingMapType)
        {
            case PathFindingMapType.Astar45:
                this.astarTypeUse = new AstarFindRoad45();
                break;

            case PathFindingMapType.Astar90:
                this.astarTypeUse = new AstarFindRoad90();
                break;

            case PathFindingMapType.AstarHoneycomb:
                this.astarTypeUse = new AstarFindRoadHoneycomb();
                break;

            case PathFindingMapType.JPS90:
                this.astarTypeUse = new JPSFindRoad90();
                break;

            default:
                break;
        }
    }

}
