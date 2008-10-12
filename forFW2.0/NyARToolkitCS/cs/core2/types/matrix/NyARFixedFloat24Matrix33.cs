using System;
using System.Collections.Generic;
using jp.nyatla.nyartoolkit.cs.core;

namespace jp.nyatla.nyartoolkit.cs.core2
{
    public class NyARFixedFloat24Matrix33 : NyARI64Matrix33
    {
        public void copyFrom(NyARDoubleMatrix33 i_matrix)
        {
            this.m00 = (long)i_matrix.m00 * 0x1000000;
            this.m01 = (long)i_matrix.m01 * 0x1000000;
            this.m02 = (long)i_matrix.m02 * 0x1000000;
            this.m10 = (long)i_matrix.m10 * 0x1000000;
            this.m11 = (long)i_matrix.m11 * 0x1000000;
            this.m12 = (long)i_matrix.m12 * 0x1000000;
            this.m20 = (long)i_matrix.m20 * 0x1000000;
            this.m21 = (long)i_matrix.m21 * 0x1000000;
            this.m22 = (long)i_matrix.m22 * 0x1000000;
            return;
        }
        public static new NyARFixedFloat24Matrix33[] createArray(int i_number)
        {
            NyARFixedFloat24Matrix33[] ret = new NyARFixedFloat24Matrix33[i_number];
            for (int i = 0; i < i_number; i++)
            {
                ret[i] = new NyARFixedFloat24Matrix33();
            }
            return ret;
        }
    }

}
