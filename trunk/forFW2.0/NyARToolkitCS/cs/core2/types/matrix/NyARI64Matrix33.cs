using System;
using System.Collections.Generic;
using System.Text;

namespace jp.nyatla.nyartoolkit.cs.core2
{
    public class NyARI64Matrix33
    {
        public long m00;
        public long m01;
        public long m02;
        public long m10;
        public long m11;
        public long m12;
        public long m20;
        public long m21;
        public long m22;
        public static NyARI64Matrix33[] createArray(int i_number)
        {
            NyARI64Matrix33[] ret = new NyARI64Matrix33[i_number];
            for (int i = 0; i < i_number; i++)
            {
                ret[i] = new NyARI64Matrix33();
            }
            return ret;
        }
    }
}
