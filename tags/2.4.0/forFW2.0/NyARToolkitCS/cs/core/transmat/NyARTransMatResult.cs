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

    /**
     * NyARTransMat戻り値専用のNyARMat
     * 
     */
    public class NyARTransMatResult : NyARDoubleMatrix34
    {
        public bool has_value = false;
        /**
         * この関数は、0-PIの間で値を返します。
         * @param o_out
         */
        public void getZXYAngle(NyARDoublePoint3d o_out)
        {
            double sina = this.m21;
            if (sina >= 1.0)
            {
                o_out.x = Math.PI / 2;
                o_out.y = 0;
                o_out.z = Math.Atan2(-this.m10, this.m00);
            }
            else if (sina <= -1.0)
            {
                o_out.x = -Math.PI / 2;
                o_out.y = 0;
                o_out.z = Math.Atan2(-this.m10, this.m00);
            }
            else
            {
                o_out.x = Math.Asin(sina);
                o_out.z = Math.Atan2(-this.m01, this.m11);
                o_out.y = Math.Atan2(-this.m20, this.m22);
            }
        }
    }

}
