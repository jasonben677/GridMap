using MapFrame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    public List<RoadNode> path { get; private set; }

    public bool isMove { get; private set; }
    public int nodeIndex { get; private set; }

    // Update is called once per frame
    private void Update()
    {
        this._MovingUpdate(Time.deltaTime);
    }


    public void SetMoveInfo(List<RoadNode> path, float speed)
    {
        this.isMove = false;

        this.nodeIndex = 0;
        
        this.path = path;

        if(path.Count > 0) this.isMove = true;
    }

    private void _MovingUpdate(float dt)
    {
        if (!this.isMove) return;

        RoadNode nextNode = this.path[this.nodeIndex];
        
        float distanceX = nextNode.cx - this.transform.position.x;
        float distanceY = nextNode.cy - this.transform.position.y;

        float speed = MapManager.GetInstance().speed * dt;
    
        if (distanceX * distanceX + distanceY * distanceY > speed * speed)
        {
            // 距離夠進行移動
            float moveAngle = Mathf.Atan2(distanceY, distanceX);

            float speedX = Mathf.Cos(moveAngle) * speed;
            float speedY = Mathf.Sin(moveAngle) * speed;

            this.transform.position += new Vector3(speedX, speedY, 0);
        }
        else
        {
            //Debug.LogWarning(string.Format("length : {0},{1}", distanceX, distanceY));

            if (this.nodeIndex == (this.path.Count - 1))
            {
                // 停止移動
                this.transform.position = new Vector3(nextNode.cx, nextNode.cy, this.transform.position.z);

                this.isMove = false;
            }
            else
            {
                // 切換節點
                this.nodeIndex++;
            }
        }

    }

}


