using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace jp.nyatla.nyartoolkit.cs.core
{
    /**
     * RGBラスタをGrayScaleに変換するフィルタを作成します。
     * このフィルタは、RGB値の平均値を、(R*G*B)/(255*255)で算出します。
     * 
     * この値は、RGB成分の作る立方体の体積を0-255スケールにした値です。
     *
     */
    public class NyARRasterFilter_Rgb2Gs_RgbCube : INyARRasterFilter_Rgb2Gs
    {
	    private IdoFilterImpl _dofilterimpl;
	    public NyARRasterFilter_Rgb2Gs_RgbCube(int i_in_raster_type)
	    {
		    if(!initInstance(i_in_raster_type,NyARBufferType.INT1D_GRAY_8))
		    {
			    throw new NyARException();
		    }
	    }
	    public NyARRasterFilter_Rgb2Gs_RgbCube(int i_in_raster_type,int i_out_raster)
	    {
		    if(!initInstance(i_in_raster_type,i_out_raster))
		    {
			    throw new NyARException();
		    }
	    }	
	    protected bool initInstance(int i_in_raster_type,int i_out_raster_type)
	    {
		    switch(i_out_raster_type){
		    case NyARBufferType.INT1D_GRAY_8:
			    switch (i_in_raster_type) {
			    case NyARBufferType.BYTE1D_B8G8R8_24:
			    case NyARBufferType.BYTE1D_R8G8B8_24:
				    this._dofilterimpl=new IdoFilterImpl_BYTE1D_B8G8R8_24();
				    break;
			    default:
				    return false;
			    }
			    break;
		    default:
			    return false;
		    }
		    return true;
	    }	
    	
    	
	    public void doFilter(INyARRgbRaster i_input, NyARGrayscaleRaster i_output)
	    {
		    Debug.Assert (i_input.getSize().isEqualSize(i_output.getSize()) == true);
		    this._dofilterimpl.doFilter(i_input,i_output,i_input.getSize());
	    }
    	
	    interface IdoFilterImpl
	    {
		    void doFilter(INyARRaster i_input, INyARRaster i_output,NyARIntSize i_size);
	    }
	    class IdoFilterImpl_BYTE1D_B8G8R8_24 : IdoFilterImpl
	    {
		    /**
		     * This function is not optimized.
		     */
		    public void doFilter(INyARRaster i_input, INyARRaster i_output,NyARIntSize i_size)
		    {
                Debug.Assert(i_input.isEqualBufferType(NyARBufferType.BYTE1D_B8G8R8_24)
					    ||	i_input.isEqualBufferType(NyARBufferType.BYTE1D_R8G8B8_24));
                Debug.Assert(i_output.isEqualBufferType(NyARBufferType.INT1D_GRAY_8));
    			
			    int[] out_buf = (int[]) i_output.getBuffer();
			    byte[] in_buf = (byte[]) i_input.getBuffer();

			    int bp = 0;
			    for (int y = 0; y < i_size.h; y++) {
				    for (int x = 0; x < i_size.w; x++) {
					    out_buf[y*i_size.w+x] = ((in_buf[bp] & 0xff) * (in_buf[bp + 1] & 0xff) * (in_buf[bp + 2] & 0xff)) >> 16;
					    bp += 3;
				    }
			    }
			    return;
		    }
	    }
    	
    }
}
