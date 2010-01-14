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
     * RGB画像をHSV画像に変換します。
     *
     */
    public class NyARRasterFilter_Rgb2Hsv : INyARRasterFilter
    {
        private IdoFilterImpl _dofilterimpl;
        public NyARRasterFilter_Rgb2Hsv(int i_raster_type)
        {
            switch (i_raster_type)
            {
                case NyARBufferType.BYTE1D_B8G8R8_24:
                    this._dofilterimpl = new IdoFilterImpl_BYTE1D_B8G8R8_24();
                    break;
                case NyARBufferType.BYTE1D_R8G8B8_24:
                default:
                    throw new NyARException();
            }
        }
        public void doFilter(INyARRaster i_input, INyARRaster i_output)
        {
            Debug.Assert(i_input.getSize().isEqualSize(i_output.getSize()) == true);
            this._dofilterimpl.doFilter(i_input, i_output, i_input.getSize());
        }

        interface IdoFilterImpl
        {
            void doFilter(INyARRaster i_input, INyARRaster i_output, NyARIntSize i_size);

        }
        class IdoFilterImpl_BYTE1D_B8G8R8_24 : IdoFilterImpl
        {
            public void doFilter(INyARRaster i_input, INyARRaster i_output, NyARIntSize i_size)
            {
                Debug.Assert(i_input.isEqualBufferType(NyARBufferType.INT1D_X7H9S8V8_32));

                int[] out_buf = (int[])i_output.getBuffer();
                byte[] in_buf = (byte[])i_input.getBuffer();
                int s;
                for (int i = i_size.h * i_size.w - 1; i >= 0; i--)
                {
                    int r = (in_buf[i * 3 + 2] & 0xff);
                    int g = (in_buf[i * 3 + 1] & 0xff);
                    int b = (in_buf[i * 3 + 0] & 0xff);
                    int cmax, cmin;
                    //最大値と最小値を計算
                    if (r > g)
                    {
                        cmax = r;
                        cmin = g;
                    }
                    else
                    {
                        cmax = g;
                        cmin = r;
                    }
                    if (b > cmax)
                    {
                        cmax = b;
                    }
                    if (b < cmin)
                    {
                        cmin = b;
                    }
                    int h;
                    if (cmax == 0)
                    {
                        s = 0;
                        h = 0;
                    }
                    else
                    {
                        s = (cmax - cmin) * 255 / cmax;
                        int cdes = cmax - cmin;
                        //H成分を計算
                        if (cdes != 0)
                        {
                            if (cmax == r)
                            {
                                h = (g - b) * 60 / cdes;
                            }
                            else if (cmax == g)
                            {
                                h = (b - r) * 60 / cdes + 2 * 60;
                            }
                            else
                            {
                                h = (r - g) * 60 / cdes + 4 * 60;
                            }
                        }
                        else
                        {
                            h = 0;
                        }
                    }
                    if (h < 0)
                    {
                        h += 360;
                    }
                    //hsv変換(h9s8v8)
                    out_buf[i] = (0x1ff0000 & (h << 16)) | (0x00ff00 & (s << 8)) | (cmax & 0xff);
                }
                return;
            }
        }
    }
}
