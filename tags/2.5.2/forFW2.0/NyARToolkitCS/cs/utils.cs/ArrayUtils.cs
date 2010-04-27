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
using System.Collections.Generic;
using System.Text;

namespace jp.nyatla.nyartoolkit.cs.utils
{/*
    public class Array2<T> where T : new()
    {
        public T[] createArray(int i_length)
        {
            T[] ret = new T[i_length];
            for (int i = 0; i < i_length; i++)
            {
                ret[i]=new T();
            }
            return ret;
        }
    }*/
    public class ArrayUtils
    {
        public static double[][] newDouble2dArray(int i_r, int i_c)
        {
            double[][] d = new double[i_r][];
            for (int i = 0; i < i_r; i++)
            {
                d[i] = new double[i_c];
            }
            return d;
        }
        public static int[][] newInt2dArray(int i_r, int i_c)
        {
            int[][] d = new int[i_r][];
            for (int i = 0; i < i_r; i++)
            {
                d[i] = new int[i_c];
            }
            return d;
        }
        public static long[][] newLong2dArray(int i_r, int i_c)
        {
            long[][] d = new long[i_r][];
            for (int i = 0; i < i_r; i++)
            {
                d[i] = new long[i_c];
            }
            return d;
        }
    }
}
