namespace jp.nyatla.nyartoolkit.cs.core
{


    public class NyARRgbRaster_BYTE1D_X8B8G8R8_32 : NyARRgbRaster
    {
        protected byte[] _buf;
        public NyARRgbRaster_BYTE1D_X8B8G8R8_32(int i_width, int i_height, boolean i_is_alloc)
        {
            super(i_width, i_height, i_is_alloc);
            this._buf = i_is_alloc ? new byte[i_width * i_height * 4] : null;

        }
        sealed public Object getBuffer()
        {
            return this._buf;
        }
        sealed public int getBufferType()
        {
            return NyARBufferType.BYTE1D_X8B8G8R8_32;
        }
        sealed public void wrapBuffer(Object i_buf)
        {
            assert(!this._is_attached_buffer);// バッファがアタッチされていたら機能しない。
            //ラスタの形式は省略。
            this._buf = (byte[])i_buf;
        }
        sealed public int[] getPixel(int i_x, int i_y, int[] o_rgb)
	{
		final byte[] ref_buf = this._buf;
		final int bp = (i_x + i_y * this._size.w) * 4;
		o_rgb[0] = (ref_buf[bp + 3] & 0xff);// R
		o_rgb[1] = (ref_buf[bp + 2] & 0xff);// G
		o_rgb[2] = (ref_buf[bp + 1] & 0xff);// B
		return o_rgb;
	}
        sealed public int[] getPixelSet(int[] i_x, int[] i_y, int i_num, int[] o_rgb)
        {
            int bp;
            const int width = this._size.w;
            const byte[] ref_buf = this._buf;
            for (int i = i_num - 1; i >= 0; i--)
            {
                bp = (i_x[i] + i_y[i] * width) * 4;
                o_rgb[i * 3 + 0] = (ref_buf[bp + 3] & 0xff);// R
                o_rgb[i * 3 + 1] = (ref_buf[bp + 2] & 0xff);// G
                o_rgb[i * 3 + 2] = (ref_buf[bp + 1] & 0xff);// B
            }
            return o_rgb;
        }
        sealed public void setPixel(int i_x, int i_y, int i_r, int i_g, int i_b)
        {
            byte[] ref_buf = this._buf;
            int bp = (i_x + i_y * this._size.w) * 4;
            ref_buf[bp + 3] = (byte)i_r;
            ref_buf[bp + 2] = (byte)i_g;
            ref_buf[bp + 1] = (byte)i_b;
        }
        sealed public void setPixel(int i_x, int i_y, int[] i_rgb)
        {
            byte[] ref_buf = this._buf;
            int bp = (i_x + i_y * this._size.w) * 4;
            ref_buf[bp + 3] = (byte)i_rgb[0];
            ref_buf[bp + 2] = (byte)i_rgb[1];
            ref_buf[bp + 1] = (byte)i_rgb[2];
        }
        sealed public void setPixels(int[] i_x, int[] i_y, int i_num, int[] i_intrgb)
        {
            byte[] ref_buf = this._buf;
            for (int i = i_num - 1; i >= 0; i--)
            {
                int bp = (i_x[i] + i_y[i] * this._size.w) * 4;
                ref_buf[bp + 3] = (byte)i_intrgb[3 * i + 0];
                ref_buf[bp + 2] = (byte)i_intrgb[3 * i + 1];
                ref_buf[bp + 1] = (byte)i_intrgb[3 * i + 2];
            }
        }
    }
}