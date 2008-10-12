using System;
using jp.nyatla.nyartoolkit.cs.core;

namespace jp.nyatla.nyartoolkit.cs.core2
{
    public class NyARFixedFloat16Point2d : NyARI64Point2d
    {
        public new static NyARFixedFloat16Point2d[] createArray(int i_number)
        {
            NyARFixedFloat16Point2d[] ret = new NyARFixedFloat16Point2d[i_number];
            for (int i = 0; i < i_number; i++)
            {
                ret[i] = new NyARFixedFloat16Point2d();
            }
            return ret;
        }
    }
}
