using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapFrame;
using Newtonsoft.Json;

public class MapManager : SingletonBase<MapManager>
{
    [SerializeField] GameObject mapPrefab;
    [SerializeField] GameObject characterPrefab;

    public float speed;

    public Mover character { get; private set; }

    public ScenceInformation scenceInformation { get; private set; }

    public Camera cam { get; private set; }


    private void Awake()
    {
        this._Initialize();
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
            RoadNode startNode = this.scenceInformation.normalMap.GetNodeFromWorld(this.character.transform.position.x, this.character.transform.position.y);
            RoadNode target = this.scenceInformation.normalMap.GetNodeFromWorld(hits[0].point.x, hits[0].point.y);

            Debug.LogWarning(string.Format("start : {0},{1}", startNode.dx, startNode.dy));
            Debug.LogWarning(string.Format("target : {0},{1}", target.dx, target.dy));

            List<RoadNode> path = this.scenceInformation.normalMap.astarTypeUse.SeekPath(startNode, target);

            this.character.SetMoveInfo(path, this.speed);
        }
        else
        {
            Debug.LogWarning("沒打到東西");
        }

    }



    private async void _Initialize()
    {
        GetInstance();

        this.cam = Camera.main;

        // 設定地圖
        this.scenceInformation = Instantiate(this.mapPrefab).GetComponent<ScenceInformation>();
        await this.scenceInformation.LoadScenceInformation();
        
        // 設定角色
        this.character = Instantiate(this.characterPrefab).GetComponent<Mover>();
        this._SetMoverToOriginPos();
    }


    private void _SetMoverToOriginPos() 
    {
        RoadNode roadNode = this.scenceInformation.normalMap.GetNodeFromGrid(0, 0);

        this.character.transform.position = new Vector2(roadNode.cx, roadNode.cy);
    }

}
