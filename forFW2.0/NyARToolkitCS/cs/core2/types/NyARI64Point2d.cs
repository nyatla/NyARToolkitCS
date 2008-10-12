using System;
using System.Collections.Generic;
using System.Text;

namespace jp.nyatla.nyartoolkit.cs.core2
{
    public class NyARI64Point2d
    {
        public long x;

        public long y;
        /**
         * 配列ファクトリ
         * @param i_number
         * @return
         */
        public static NyARI64Point2d[] createArray(int i_number)
        {
            NyARI64Point2d[] ret = new NyARI64Point2d[i_number];
            for (int i = 0; i < i_number; i++)
            {
                ret[i] = new NyARI64Point2d();
            }
            return ret;
        }
    }
}
