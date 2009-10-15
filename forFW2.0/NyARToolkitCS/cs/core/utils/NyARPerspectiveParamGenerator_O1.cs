/* 
 * PROJECT: NyARToolkitCS(Extension)
 * --------------------------------------------------------------------------------
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

namespace jp.nyatla.nyartoolkit.cs.core
{
    /**
     * NyARPerspectiveParamGeneratorを最適化したクラスです。
     */
    public class NyARPerspectiveParamGenerator_O1 : NyARPerspectiveParamGenerator
    {
        public NyARPerspectiveParamGenerator_O1(int i_local_x, int i_local_y, int i_width, int i_height)
            : base(i_local_x, i_local_y, i_width, i_height)
        {
            this._height = i_height;
            this._width = i_width;
            this._local_x = i_local_x;
            this._local_y = i_local_y;
            return;
        }
        public override bool getParam(NyARIntPoint2d[] i_vertex, double[] o_param)
        {
            double G, H;
            double w1, w2, w3, w4;
            double x0 = i_vertex[0].x;
            double x1 = i_vertex[1].x;
            double x2 = i_vertex[2].x;
            double x3 = i_vertex[3].x;
            double y0 = i_vertex[0].y;
            double y1 = i_vertex[1].y;
            double y2 = i_vertex[2].y;
            double y3 = i_vertex[3].y;
            double ltx = this._local_x;
            double lty = this._local_y;
            double rbx = ltx + this._width;
            double rby = lty + this._height;


            w1 = -y3 + y0;
            w2 = y2 - y1;
            double la2_33 = ltx * w1 + rbx * w2;//これが0になるのはまずい。
            double la2_34 = (rby * (-y3 + y2) + lty * (y0 - y1)) / la2_33;
            double ra2_3 = (-w1 - w2) / la2_33;

            w1 = -x3 + x0;
            w2 = x2 - x1;
            double la1_33 = ltx * w1 + rbx * w2;//これが0になるのはまずい。

            //GHを計算
            H = (ra2_3 - ((-w1 - w2) / la1_33)) / (la2_34 - ((rby * (-x3 + x2) + lty * (x0 - x1)) / la1_33));
            G = ra2_3 - la2_34 * H;
            o_param[7] = H;
            o_param[6] = G;

            //残りを計算
            w3 = rby - lty;
            w4 = rbx - ltx;
            w1 = (y2 - y1 - H * (-rby * y2 + lty * y1) - G * (-rbx * y2 + rbx * y1)) / w3;
            w2 = (y1 - y0 - H * (-lty * y1 + lty * y0) - G * (-rbx * y1 + ltx * y0)) / w4;
            o_param[5] = y0 * (1 + H * lty + G * ltx) - w1 * lty - w2 * ltx;
            o_param[4] = w1;
            o_param[3] = w2;


            w1 = (x2 - x1 - H * (-rby * x2 + lty * x1) - G * (-rbx * x2 + rbx * x1)) / w3;
            w2 = (x1 - x0 - H * (-lty * x1 + lty * x0) - G * (-rbx * x1 + ltx * x0)) / w4;
            o_param[2] = x0 * (1 + H * lty + G * ltx) - w1 * lty - w2 * ltx;
            o_param[1] = w1;
            o_param[0] = w2;
            return true;
        }
    }

}
