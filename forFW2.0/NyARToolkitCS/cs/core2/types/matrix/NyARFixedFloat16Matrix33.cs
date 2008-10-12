using System;
using jp.nyatla.nyartoolkit.cs.core;

namespace jp.nyatla.nyartoolkit.cs.core2
{
    public class NyARFixedFloat16Matrix33 : NyARI64Matrix33
    {
        public void copyFrom(NyARDoubleMatrix33 i_matrix)
        {
            this.m00 = (long)i_matrix.m00 * 0x10000;
            this.m01 = (long)i_matrix.m01 * 0x10000;
            this.m02 = (long)i_matrix.m02 * 0x10000;
            this.m10 = (long)i_matrix.m10 * 0x10000;
            this.m11 = (long)i_matrix.m11 * 0x10000;
            this.m12 = (long)i_matrix.m12 * 0x10000;
            this.m20 = (long)i_matrix.m20 * 0x10000;
            this.m21 = (long)i_matrix.m21 * 0x10000;
            this.m22 = (long)i_matrix.m22 * 0x10000;
            return;
        }
        public static new NyARFixedFloat16Matrix33[] createArray(int i_number)
        {
            NyARFixedFloat16Matrix33[] ret = new NyARFixedFloat16Matrix33[i_number];
            for (int i = 0; i < i_number; i++)
            {
                ret[i] = new NyARFixedFloat16Matrix33();
            }
            return ret;
        }
    }
}
