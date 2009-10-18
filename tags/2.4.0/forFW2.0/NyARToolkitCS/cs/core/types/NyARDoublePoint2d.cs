/* 
 * PROJECT: NyARToolkitCS
 * --------------------------------------------------------------------------------
 * This work is based on the original ARToolKit developed by
 *   Hirokazu Kato
 *   Mark Billinghurst
 *   HITLab, University of Washington, Seattle
 * http://www.hitl.washington.edu/artoolkit/
 *
 * The NyARToolkitCS is C# edition ARToolKit class library.
 * Copyright (C)2008-2009 Ryo Iizuka
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * For further information please contact.
 *	http://nyatla.jp/nyatoolkit/
 *	<airmail(at)ebony.plala.or.jp> or <nyatla(at)nyatla.jp>
 * 
 */
using System;

namespace jp.nyatla.nyartoolkit.cs.core
{
    public class NyARDoublePoint2d
    {
        public double x;
        public double y;
        /**
         * 配列ファクトリ
         * @param i_number
         * @return
         */
        public static NyARDoublePoint2d[] createArray(int i_number)
        {
            NyARDoublePoint2d[] ret = new NyARDoublePoint2d[i_number];
            for (int i = 0; i < i_number; i++)
            {
                ret[i] = new NyARDoublePoint2d();
            }
            return ret;
        }
        public NyARDoublePoint2d()
        {
            this.x = 0;
            this.y = 0;
            return;
        }
        public NyARDoublePoint2d(double i_x, double i_y)
        {
            this.x = i_x;
            this.y = i_y;
            return;
        }
        public NyARDoublePoint2d(NyARDoublePoint2d i_src)
        {
            this.x = i_src.x;
            this.y = i_src.y;
            return;
        }
        public NyARDoublePoint2d(NyARIntPoint2d i_src)
        {
            this.x = (double)i_src.x;
            this.y = (double)i_src.y;
            return;
        }
        public void setValue(NyARDoublePoint2d i_src)
        {
            this.x = i_src.x;
            this.y = i_src.y;
            return;
        }
        public void setValue(NyARIntPoint2d i_src)
        {
            this.x = (double)i_src.x;
            this.y = (double)i_src.y;
            return;
        }
        /**
         * 格納値をベクトルとして、距離を返します。
         * @return
         */
        public double dist()
        {
            return Math.Sqrt(this.x * this.x + this.y + this.y);
        }
    }
}
