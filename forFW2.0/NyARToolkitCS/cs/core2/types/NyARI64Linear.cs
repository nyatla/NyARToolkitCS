using System;
using System.Collections.Generic;
using System.Text;

namespace jp.nyatla.nyartoolkit.cs.core2
{
    public class NyARI64Linear
    {
        public long rise;//y軸の増加量
        public long run;//x軸の増加量
        public long intercept;//切片
        public void copyFrom(NyARI64Linear i_source)
        {
            this.rise = i_source.rise;
            this.run = i_source.run;
            this.intercept = i_source.intercept;
            return;
        }
        public static NyARI64Linear[] createArray(int i_number)
        {
            NyARI64Linear[] ret = new NyARI64Linear[i_number];
            for (int i = 0; i < i_number; i++)
            {
                ret[i] = new NyARI64Linear();
            }
            return ret;
        }
    }
}
