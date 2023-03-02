using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapFrame
{
    public class RoadNode
    {
        /// <summary>
        /// 世界坐標x
        /// </summary>
        public float cx;

        /// <summary>
        /// 世界坐標y
        /// </summary>
        public float cy;

        /// <summary>
        /// grid x座標
        /// </summary>
        public int dx;

        /// <summary>
        /// grid y座標
        /// </summary>
        public int dy;

        /// <summary>
        /// 節點狀態
        /// </summary>
        public int value;

        /// <summary>
        /// 總和
        /// </summary>
        public float f 
        {
            get 
            {
                return g + h;
            }
        }

        /// <summary>
        /// 起始點到目前節點的距離
        /// </summary>
        public float g;

        /// <summary>
        /// 目前節點到終點距離
        /// </summary>
        public float h;

        /// <summary>
        /// 節點的父節點
        /// </summary>
        public RoadNode parent;

        public float drawOrder;

        public RoadNode()
        {

        }

        public RoadNode(RoadNode target, RoadNode parent)
        {
            this.cx = target.cx;
            this.cy = target.cy;
            this.dx = target.dx;
            this.dy = target.dy;
            this.value = target.value;
            this.parent = parent;
        }

        public override string ToString()
        {
            return "世界坐標：（" + this.cx + "," + this.cy + "),  " +
                "本地網格坐標：（" + this.dx + "," + this.dy + ")" +
                "value: " + this.value + " f: " + this.f + " g: " + this.g + " h: " + this.h +
                "drawOrder: " + this.drawOrder;
        }
    }
}

