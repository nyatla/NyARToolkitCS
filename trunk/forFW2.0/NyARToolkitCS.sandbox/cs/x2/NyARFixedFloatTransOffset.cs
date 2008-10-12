using System;
using jp.nyatla.nyartoolkit.cs.core;
using jp.nyatla.nyartoolkit.cs.core2;

namespace jp.nyatla.nyartoolkit.cs.sandbox.x2
{
    public class NyARFixedFloatTransOffset
    {
        public NyARFixedFloat16Point3d[] vertex = NyARFixedFloat16Point3d.createArray(4);
        public NyARFixedFloat16Point3d point = new NyARFixedFloat16Point3d();
        /**
         * 中心位置と辺長から、オフセット情報を作成して設定する。
         * @param i_width
         * FF16で渡すこと！
         * @param i_center
         */
        public void setSquare(long i_width, NyARFixedFloat16Point2d i_center)
        {
            long w_2 = i_width >> 1;

            NyARFixedFloat16Point3d vertex3d_ptr;
            vertex3d_ptr = this.vertex[0];
            vertex3d_ptr.x = -w_2;
            vertex3d_ptr.y = w_2;
            vertex3d_ptr.z = 0;
            vertex3d_ptr = this.vertex[1];
            vertex3d_ptr.x = w_2;
            vertex3d_ptr.y = w_2;
            vertex3d_ptr.z = 0;
            vertex3d_ptr = this.vertex[2];
            vertex3d_ptr.x = w_2;
            vertex3d_ptr.y = -w_2;
            vertex3d_ptr.z = 0;
            vertex3d_ptr = this.vertex[3];
            vertex3d_ptr.x = -w_2;
            vertex3d_ptr.y = -w_2;
            vertex3d_ptr.z = 0;

            this.point.x = -i_center.x;
            this.point.y = -i_center.y;
            this.point.z = 0;
            return;
        }
    }
}