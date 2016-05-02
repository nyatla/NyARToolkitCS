namespace jp.nyatla.nyartoolkit.cs.core
{

    public class NyARRgbRaster_BYTE1D_B8G8R8_24 : NyARRgbRaster
    {

        protected byte[] _buf;
        public NyARRgbRaster_BYTE1D_B8G8R8_24(int i_width, int i_height, boolean i_is_alloc)
        {
            super(i_width, i_height, i_is_alloc);
            this._buf = i_is_alloc ? new byte[i_width * i_height * 3] : null;
        }

        sealed public Object getBuffer()
        {
            return this._buf;
        }

        sealed public int getBufferType()
        {
            return NyARBufferType.BYTE1D_B8G8R8_24;
        }

        sealed public void wrapBuffer(Object i_buf)
        {
            assert(!this._is_attached_buffer);// バッファがアタッチされていたら機能しない。
            //ラスタの形式は省略。
            this._buf = (byte[])i_buf;
        }

        sealed public int[] getPixel(int i_x, int i_y, int[] o_rgb)
        {
            const byte[] ref_buf = this._buf;
            const int bp = (i_x + i_y * this._size.w) * 3;
            o_rgb[0] = (ref_buf[bp + 2] & 0xff);// R
            o_rgb[1] = (ref_buf[bp + 1] & 0xff);// G
            o_rgb[2] = (ref_buf[bp + 0] & 0xff);// B
            return o_rgb;
        }

        sealed public int[] getPixelSet(int[] i_x, int[] i_y, int i_num, int[] o_rgb)
        {
            int bp;
            const int width = this._size.w;
            const byte[] ref_buf = this._buf;
            for (int i = i_num - 1; i >= 0; i--)
            {
                bp = (i_x[i] + i_y[i] * width) * 3;
                o_rgb[i * 3 + 0] = (ref_buf[bp + 2] & 0xff);// R
                o_rgb[i * 3 + 1] = (ref_buf[bp + 1] & 0xff);// G
                o_rgb[i * 3 + 2] = (ref_buf[bp + 0] & 0xff);// B
            }
            return o_rgb;
        }

        sealed public void setPixel(int i_x, int i_y, int i_r, int i_g, int i_b)
        {
            const byte[] ref_buf = this._buf;
            const int idx = (i_y * this._size.w + i_x) * 3;
            ref_buf[idx + 0] = (byte)i_b;// B
            ref_buf[idx + 1] = (byte)i_g;// G
            ref_buf[idx + 2] = (byte)i_r;// R
        }

        sealed public void setPixel(int i_x, int i_y, int[] i_rgb)
        {
            const byte[] ref_buf = this._buf;
            const int idx = (i_y * this._size.w + i_x) * 3;
            ref_buf[idx + 0] = (byte)i_rgb[2];// B
            ref_buf[idx + 1] = (byte)i_rgb[1];// G
            ref_buf[idx + 2] = (byte)i_rgb[0];// R
        }

        sealed public void setPixels(int[] i_x, int[] i_y, int i_num, int[] i_intrgb)
        {
            NyARRuntimeException.notImplement();
        }

    }
}
