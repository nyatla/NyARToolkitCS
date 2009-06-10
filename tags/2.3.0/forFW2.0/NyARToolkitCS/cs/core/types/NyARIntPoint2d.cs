using System;
using System.Collections.Generic;
using System.Text;

namespace jp.nyatla.nyartoolkit.cs.core
{
    public class NyARIntPoint2d
    {
        public int x;

        public int y;
        /**
         * 配列ファクトリ
         * @param i_number
         * @return
         */
        public static NyARIntPoint2d[] createArray(int i_number)
        {
            NyARIntPoint2d[] ret = new NyARIntPoint2d[i_number];
            for (int i = 0; i < i_number; i++)
            {
                ret[i] = new NyARIntPoint2d();
            }
            return ret;
        }
        public static void copyArray(NyARIntPoint2d[] i_from, NyARIntPoint2d[] i_to)
        {
            for (int i = i_from.Length - 1; i >= 0; i--)
            {
                i_to[i].x = i_from[i].x;
                i_to[i].y = i_from[i].y;
            }
            return;
        }
    }
}
