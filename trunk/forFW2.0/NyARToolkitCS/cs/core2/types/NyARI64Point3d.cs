using System;

namespace jp.nyatla.nyartoolkit.cs.core2
{
    public class NyARI64Point3d
    {
        public long x;
        public long y;
        public long z;
        /**
         * 配列ファクトリ
         * @param i_number
         * @return
         */
        public static NyARI64Point3d[] createArray(int i_number)
        {
            NyARI64Point3d[] ret = new NyARI64Point3d[i_number];
            for (int i = 0; i < i_number; i++)
            {
                ret[i] = new NyARI64Point3d();
            }
            return ret;
        }
    }
}
