using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace jp.nyatla.nyartoolkit.cs.core
{
    public class NyARRgbRaster : NyARRgbRaster_BasicClass
    {
	    protected object _buf;
	    protected INyARRgbPixelReader _reader;
	    /**
	     * バッファオブジェクトがアタッチされていればtrue
	     */
	    protected bool _is_attached_buffer;
	    public NyARRgbRaster(int i_width, int i_height,int i_raster_type,bool i_is_alloc)
            : base(i_width, i_height, i_raster_type)
	    {
		    if(!initInstance(this._size,i_raster_type,i_is_alloc)){
			    throw new NyARException();
		    }
	    }
	    public NyARRgbRaster(int i_width, int i_height,int i_raster_type)
            : base(i_width, i_height, i_raster_type)
	    {
		    if(!initInstance(this._size,i_raster_type,true)){
			    throw new NyARException();
		    }
	    }
	    protected bool initInstance(NyARIntSize i_size,int i_raster_type,bool i_is_alloc)
	    {
		    switch(i_raster_type)
		    {
			    case NyARBufferType.INT1D_X8R8G8B8_32:
				    this._buf=i_is_alloc?new int[i_size.w*i_size.h]:null;
				    this._reader=new NyARRgbPixelReader_INT1D_X8R8G8B8_32((int[])this._buf,i_size);
				    break;
			    case NyARBufferType.BYTE1D_B8G8R8X8_32:
				    this._buf=i_is_alloc?new byte[i_size.w*i_size.h*4]:null;
				    this._reader=new NyARRgbPixelReader_BYTE1D_B8G8R8X8_32((byte[])this._buf,i_size);
				    break;
			    case NyARBufferType.BYTE1D_R8G8B8_24:
				    this._buf=i_is_alloc?new byte[i_size.w*i_size.h*3]:null;
				    this._reader=new NyARRgbPixelReader_BYTE1D_R8G8B8_24((byte[])this._buf,i_size);
				    break;
			    default:
				    return false;
		    }
		    this._is_attached_buffer=i_is_alloc;
		    return true;
	    }
	    public override INyARRgbPixelReader getRgbPixelReader()
	    {
		    return this._reader;
	    }
        public override object getBuffer()
	    {
		    return this._buf;
	    }
        public override bool hasBuffer()
	    {
		    return this._buf!=null;
	    }
        public override void wrapBuffer(object i_ref_buf)
	    {
		    Debug.Assert(!this._is_attached_buffer);//バッファがアタッチされていたら機能しない。
		    this._buf=i_ref_buf;
		    //ピクセルリーダーの参照バッファを切り替える。
		    this._reader.switchBuffer(i_ref_buf);
	    }
    }
}
