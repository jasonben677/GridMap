
using System;

namespace toinfiniityandbeyond.Tilemapping
{
    public static class MapEditorDefine
    {
        // 地圖跟unity的比例尺
        public static float scale = 100f;

        // 限制每禎渲染的數量
        public static int renderPerFrame = 1500;

        public static float ChangeToEditorSize(float origin)
        {
            return (origin /scale);
        }


     }
}

