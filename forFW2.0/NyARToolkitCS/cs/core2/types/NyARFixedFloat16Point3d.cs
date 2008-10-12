using System;
using jp.nyatla.nyartoolkit.cs.core;

namespace jp.nyatla.nyartoolkit.cs.core2
{
    public class NyARFixedFloat16Point3d : NyARI64Point3d
    {
        /**
         * 配列ファクトリ
         * @param i_number
         * @return
         */
        public new static NyARFixedFloat16Point3d[] createArray(int i_number)
        {
            NyARFixedFloat16Point3d[] ret = new NyARFixedFloat16Point3d[i_number];
            for (int i = 0; i < i_number; i++)
            {
                ret[i] = new NyARFixedFloat16Point3d();
            }
            return ret;
        }
    }
}
