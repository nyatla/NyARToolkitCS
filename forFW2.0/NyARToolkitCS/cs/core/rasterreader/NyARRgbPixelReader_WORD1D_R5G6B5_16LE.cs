using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
namespace jp.nyatla.nyartoolkit.cs.core
{
    /**
     * 主にWindowsMobileのRGB565フォーマット読み出し用
     */
    public class NyARRgbPixelReader_WORD1D_R5G6B5_16LE : INyARRgbPixelReader
    {
	    protected short[] _ref_buf;

	    private NyARIntSize _size;

        public NyARRgbPixelReader_WORD1D_R5G6B5_16LE(short[] i_buf, NyARIntSize i_size)
	    {
		    this._ref_buf = i_buf;
		    this._size = i_size;
	    }

	    public void getPixel(int i_x, int i_y, int[] o_rgb)
	    {
            short[] buf = this._ref_buf;
            int y = i_y;
            int idx = y * this._size.w + i_x;
            int pixcel =(int)(buf[idx] &0xffff);

            o_rgb[0] = (int)((pixcel & 0xf800) >> 8);//R
            o_rgb[1] = (int)((pixcel & 0x07e0) >> 3);//G
            o_rgb[2] = (int)((pixcel & 0x001f) << 3);//B
		    return;
	    }

	    public void getPixelSet(int[] i_x, int[] i_y, int i_num, int[] o_rgb)
	    {
            int stride = this._size.w;
            short[] buf = this._ref_buf;

            for (int i = i_num - 1; i >= 0; i--)
            {
                int idx = i_y[i] * stride + i_x[i];

                int pixcel =(int)(buf[idx] &0xffff);
                o_rgb[i * 3 + 0] = (int)((pixcel & 0xf800) >> 8);//R
                o_rgb[i * 3 + 1] = (int)((pixcel & 0x07e0) >> 3);//G
                o_rgb[i * 3 + 2] = (int)((pixcel & 0x001f) << 3);//B
            }		
		    return;
	    }
	    public void setPixel(int i_x, int i_y, int[] i_rgb)
	    {
		    NyARException.notImplement();		
	    }
	    public void setPixel(int i_x, int i_y, int i_r,int i_g,int i_b)
	    {
		    NyARException.notImplement();		
	    }
    	
	    public void setPixels(int[] i_x, int[] i_y, int i_num, int[] i_intrgb)
	    {
		    NyARException.notImplement();		
	    }
	    public void switchBuffer(Object i_ref_buffer)
	    {
            Debug.Assert(((short[])i_ref_buffer).Length >= this._size.w * this._size.h);
		    this._ref_buf=(short[])i_ref_buffer;
	    }
    }
}
