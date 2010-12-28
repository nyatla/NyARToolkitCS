using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Drawing;
using jp.nyatla.nyartoolkit.cs;
using jp.nyatla.nyartoolkit.cs.core;

namespace NyARToolkitCSUtils
{
    /**
     * 特定のピクセルフォーマットと互換性のあるバッファを持つNyARRasterです。
     * 初期化時にPixelFormatを指定します。
     */
    public class NyARBitmapRaster : NyARRgbRaster
    {
        private static int pixelFormat2BufType(PixelFormat pixel_formet)
        {
            switch(pixel_formet){
            case PixelFormat.Format24bppRgb:
                return NyARBufferType.BYTE1D_B8G8R8_24;
            case PixelFormat.Format32bppRgb:
            case PixelFormat.Format32bppPArgb:
            case PixelFormat.Format32bppArgb:
                return NyARBufferType.BYTE1D_B8G8R8X8_32;
            default:
                throw new NyARException();
            }
        }
        private PixelFormat _pixel_format;
        public NyARBitmapRaster(int i_width,int i_height,PixelFormat pixel_formet)
            :base(i_width,i_height,pixelFormat2BufType(pixel_formet),false)
        {
            this._pixel_format = pixel_formet;
		    switch(this.getBufferType())
		    {
		    case NyARBufferType.BYTE1D_B8G8R8_24:{
			    this._buf=new byte[3*i_width*i_height];
			    break;
            }
            case NyARBufferType.BYTE1D_B8G8R8X8_32:
                {
			    this._buf=new byte[4*i_width*i_height];
			    break;
            }
		    default:
			    //このデータタイプのラスタは作れない。
			    throw new NyARException();
		    }
		    //内部参照に切り替える。
            this.wrapBuffer(this._buf);
		    this._is_attached_buffer=true;
		    return;
        }
        public void setBitmapData(BitmapData i_bmpdata)
        {
            byte[] b=(byte[])this._buf;
            //一括転送
            Marshal.Copy((IntPtr)((int)i_bmpdata.Scan0), (byte[])this._buf,0,b.Length);
        }
    }
}
