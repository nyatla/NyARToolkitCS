/* 
 * PROJECT: NyARToolkitCS
 * --------------------------------------------------------------------------------
 * This work is based on the original ARToolKit developed by
 *   Hirokazu Kato
 *   Mark Billinghurst
 *   HITLab, University of Washington, Seattle
 * http://www.hitl.washington.edu/artoolkit/
 *
 * The NyARToolkit is Java version ARToolkit class library.
 * Copyright (C)2008 R.Iizuka
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this framework; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 * 
 * For further information please contact.
 *	http://nyatla.jp/nyatoolkit/
 *	<airmail(at)ebony.plala.or.jp>
 * 
 */
using System;
using System.Collections.Generic;
using System.Text;
using jp.nyatla.nyartoolkit.cs.core2;
using jp.nyatla.nyartoolkit.cs.core;

namespace jp.nyatla.nyartoolkit.cs.sandbox.x2
{
    public class NyARFixedFloatIdeal2Observ
    {
        private double[] _factor = new double[4];
        public NyARFixedFloatIdeal2Observ(NyARCameraDistortionFactor i_distfactor)
        {
            i_distfactor.getValue(this._factor);
            return;
        }
        public void ideal2ObservBatch(NyARDoublePoint2d[] i_in, NyARFixedFloat16Point2d[] o_out, int i_size)
	{
		double x, y;
		double d0 = this._factor[0];
		double d1 = this._factor[1];
		double d3 = this._factor[3];
		double d2_w = this._factor[2] / 100000000.0;
		for (int i = 0; i < i_size; i++) {
			x = (i_in[i].x - d0) * d3;
			y = (i_in[i].y - d1) * d3;
			if (x == 0.0 && y == 0.0) {
				o_out[i].x = (long)(d0*NyMath.FIXEDFLOAT16_1);
				o_out[i].y = (long)(d1*NyMath.FIXEDFLOAT16_1);
			} else {
				double d = 1.0 - d2_w * (x * x + y * y);
				o_out[i].x = (long)((x * d + d0)*NyMath.FIXEDFLOAT16_1);
				o_out[i].y = (long)((y * d + d1)*NyMath.FIXEDFLOAT16_1);
			}
		}
		return;
	}
        public void ideal2Observ(NyARFixedFloat16Point2d i_in, NyARFixedFloat16Point2d o_out)
        {
            double f0 = this._factor[0];
            double f1 = this._factor[1];
            double x = (((double)i_in.x / NyMath.FIXEDFLOAT16_1) - f0) * this._factor[3];
            double y = (((double)i_in.y / NyMath.FIXEDFLOAT16_1) - f1) * this._factor[3];
            if (x == 0.0 && y == 0.0)
            {
                o_out.x = (long)(f0 * NyMath.FIXEDFLOAT16_1);
                o_out.y = (long)(f1 * NyMath.FIXEDFLOAT16_1);
            }
            else
            {
                double d = 1.0 - this._factor[2] / 100000000.0 * (x * x + y * y);
                o_out.x = (long)((x * d + f0) * NyMath.FIXEDFLOAT16_1);
                o_out.y = (long)((y * d + f1) * NyMath.FIXEDFLOAT16_1);
            }
            return;
        }
    }
}
