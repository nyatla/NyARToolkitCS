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
using System.Diagnostics;

namespace jp.nyatla.nyartoolkit.cs.core
{
    /**
     * ネガポジ反転フィルタ。
     * 画像の明暗を反転します。
     *
     */
    public class NyARRasterFilter_Reverse : INyARRasterFilter
    {
        private IdoFilterImpl _do_filter_impl;
        public NyARRasterFilter_Reverse(int i_raster_type)
        {
            switch (i_raster_type)
            {
                case NyARBufferType.INT1D_GRAY_8:
                    this._do_filter_impl = new IdoFilterImpl_GRAY_8();
                    break;
                default:
                    throw new NyARException();
            }
        }
        public void doFilter(INyARRaster i_input, INyARRaster i_output)
        {
            this._do_filter_impl.doFilter(i_input, i_output, i_input.getSize());
        }

        interface IdoFilterImpl
        {
            void doFilter(INyARRaster i_input, INyARRaster i_output, NyARIntSize i_size);
        }
        class IdoFilterImpl_GRAY_8 : IdoFilterImpl
        {
            public void doFilter(INyARRaster i_input, INyARRaster i_output, NyARIntSize i_size)
            {
                Debug.Assert(i_input.isEqualBufferType(NyARBufferType.INT1D_GRAY_8));
                Debug.Assert(i_output.isEqualBufferType(NyARBufferType.INT1D_GRAY_8));
                int[] in_ptr = (int[])i_input.getBuffer();
                int[] out_ptr = (int[])i_output.getBuffer();


                int number_of_pixel = i_size.h * i_size.w;
                for (int i = 0; i < number_of_pixel; i++)
                {
                    out_ptr[i] = 255 - in_ptr[i];
                }
                return;
            }
        }
    }

}
