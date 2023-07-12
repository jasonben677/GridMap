using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapFrame;
using Newtonsoft.Json;

public class MapManager : MonoBehaviour
{
    [SerializeField] private TextAsset mapJson;
    [SerializeField] private Mover mover;

    [SerializeField] float speed;

    public NormalMap normalMap { get; private set; }

    public Camera cam { get; private set; }


    private async void Awake()
    {
        this.cam = Camera.main;

        this.normalMap = new NormalMap();
        this.normalMap.mapData = JsonConvert.DeserializeObject<MapData>(mapJson.text);
        await this.normalMap.SetGridData(this.normalMap.mapData);
        this._SetMoverToOriginPos();
    }    
   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            this._RaycastToScreen();
        }
    }


    private void _RaycastToScreen()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction, 500);

        if (hits.Length > 0)
        {
            RoadNode startNode = this.normalMap.GetNodeFromWorld(this.mover.transform.position.x, this.mover.transform.position.y);
            RoadNode target = this.normalMap.GetNodeFromWorld(hits[0].point.x, hits[0].point.y);

            Debug.LogWarning(string.Format("start : {0},{1}", startNode.dx, startNode.dy));
            Debug.LogWarning(string.Format("target : {0},{1}", target.dx, target.dy));

            List<RoadNode> path = this.normalMap.astarTypeUse.SeekPath(startNode, target);

            this.mover.SetMoveInfo(path, this.speed);
        }
        else
        {
            Debug.LogWarning("沒打到東西");
        }

    }


    private void _SetMoverToOriginPos() 
    {
        RoadNode roadNode = this.normalMap.GetNodeFromGrid(0, 0);

        this.mover.transform.position = new Vector2(roadNode.cx, roadNode.cy);
    }

}
