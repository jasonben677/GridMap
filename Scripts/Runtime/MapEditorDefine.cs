
using System;

namespace toinfiniityandbeyond.Tilemapping
{
    public static class MapEditorDefine
    {
        // �a�ϸ�unity����Ҥ�
        public static float scale = 100f;

        // ����C�մ�V���ƶq
        public static int renderPerFrame = 1500;

        public static float ChangeToEditorSize(float origin)
        {
            return (origin /scale);
        }


     }
}

