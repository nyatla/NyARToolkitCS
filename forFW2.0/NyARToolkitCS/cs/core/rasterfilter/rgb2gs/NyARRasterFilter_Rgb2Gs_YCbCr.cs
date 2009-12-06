using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace jp.nyatla.nyartoolkit.cs.core
{
    /**
     * YCbCr変換して、Y成分のグレースケールの値を計算します。
     * 変換式は、http://www.tyre.gotdns.org/を参考にしました。
     */
    public class NyARRasterFilter_Rgb2Gs_YCbCr : INyARRasterFilter_RgbToGs
    {
        private IdoFilterImpl _dofilterimpl;
        public NyARRasterFilter_Rgb2Gs_YCbCr(int i_raster_type)
        {
            switch (i_raster_type)
            {
                case INyARBufferReader.BUFFERFORMAT_BYTE1D_B8G8R8_24:
                    this._dofilterimpl = new IdoFilterImpl_BYTE1D_B8G8R8_24();
                    break;
                case INyARBufferReader.BUFFERFORMAT_BYTE1D_R8G8B8_24:
                default:
                    throw new NyARException();
            }
        }
        public void doFilter(INyARRgbRaster i_input, NyARGrayscaleRaster i_output)
        {
            Debug.Assert(i_input.getSize().isEqualSize(i_output.getSize()) == true);
            this._dofilterimpl.doFilter(i_input.getBufferReader(), i_output.getBufferReader(), i_input.getSize());
        }

        interface IdoFilterImpl
        {
            void doFilter(INyARBufferReader i_input, INyARBufferReader i_output, NyARIntSize i_size);
        }
        class IdoFilterImpl_BYTE1D_B8G8R8_24 : IdoFilterImpl
        {
            /**
             * This function is not optimized.
             */
            public void doFilter(INyARBufferReader i_input, INyARBufferReader i_output, NyARIntSize i_size)
            {
                Debug.Assert(i_input.isEqualBufferType(INyARBufferReader.BUFFERFORMAT_BYTE1D_B8G8R8_24));

                int[] out_buf = (int[])i_output.getBuffer();
                byte[] in_buf = (byte[])i_input.getBuffer();

                int bp = 0;
                for (int y = 0; y < i_size.h; y++)
                {
                    for (int x = 0; x < i_size.w; x++)
                    {
                        out_buf[y * i_size.w + x] = (306 * (in_buf[bp + 2] & 0xff) + 601 * (in_buf[bp + 1] & 0xff) + 117 * (in_buf[bp + 0] & 0xff)) >> 10;
                        bp += 3;
                    }
                }
                return;
            }
        }
    }
}
