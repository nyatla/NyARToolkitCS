/* 
 * PROJECT: NyARToolkitCS
 * --------------------------------------------------------------------------------
 * This work is based on the original ARToolKit developed by
 *   Hirokazu Kato
 *   Mark Billinghurst
 *   HITLab, University of Washington, Seattle
 * http://www.hitl.washington.edu/artoolkit/
 *
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
using System.Diagnostics;
namespace jp.nyatla.nyartoolkit.cs.core
{
    public class NyARRasterFilter_ARToolkitThreshold : INyARRasterFilter_Rgb2Bin
    {
	    protected int _threshold;
	    private IdoThFilterImpl _do_threshold_impl;

	    public NyARRasterFilter_ARToolkitThreshold(int i_threshold,int i_in_raster_type)
	    {
		    if(!initInstance(i_threshold,i_in_raster_type,NyARBufferType.INT1D_BIN_8)){
			    throw new NyARException();
		    }
	    }
	    public NyARRasterFilter_ARToolkitThreshold(int i_threshold,int i_in_raster_type,int i_out_raster_type)
	    {
		    if(!initInstance(i_threshold,i_in_raster_type,i_out_raster_type)){
			    throw new NyARException();
		    }
	    }
	    protected bool initInstance(int i_threshold,int i_in_raster_type,int i_out_raster_type)
	    {
		    switch(i_out_raster_type){
		    case NyARBufferType.INT1D_BIN_8:
			    switch (i_in_raster_type){
			    case NyARBufferType.BYTE1D_B8G8R8_24:
			    case NyARBufferType.BYTE1D_R8G8B8_24:
				    this._do_threshold_impl=new doThFilterImpl_BUFFERFORMAT_BYTE1D_RGB_24();
				    break;
			    case NyARBufferType.BYTE1D_B8G8R8X8_32:
				    this._do_threshold_impl=new doThFilterImpl_BUFFERFORMAT_BYTE1D_B8G8R8X8_32();
				    break;
			    case NyARBufferType.BYTE1D_X8R8G8B8_32:
				    this._do_threshold_impl=new doThFilterImpl_BUFFERFORMAT_BYTE1D_X8R8G8B8_32();
				    break;
			    case NyARBufferType.INT1D_X8R8G8B8_32:
				    this._do_threshold_impl=new doThFilterImpl_BUFFERFORMAT_INT1D_X8R8G8B8_32();
				    break;
			    case NyARBufferType.WORD1D_R5G6B5_16LE:
				    this._do_threshold_impl=new doThFilterImpl_BUFFERFORMAT_WORD1D_R5G6B5_16LE();
				    break;
			    default:
				    return false;//サポートしない組み合わせ
			    }
			    break;
		    default:
			    return false;//サポートしない組み合わせ
		    }
		    this._threshold = i_threshold;
		    return true;
	    }	
    	
	    /**
	     * 画像を２値化するための閾値。暗点<=th<明点となります。
	     * @param i_threshold
	     */
	    public void setThreshold(int i_threshold)
	    {
		    this._threshold = i_threshold;
	    }

	    public void doFilter(INyARRgbRaster i_input, NyARBinRaster i_output)
	    {

		    Debug.Assert (i_input.getSize().isEqualSize(i_output.getSize()) == true);
		    this._do_threshold_impl.doThFilter(i_input,i_output,i_output.getSize(), this._threshold);
		    return;
	    }
	    /*
	     * ここから各ラスタ用のフィルタ実装
	     */
	    interface IdoThFilterImpl
	    {
		    void doThFilter(INyARRaster i_input, INyARRaster i_output,NyARIntSize i_size,int i_threshold);
	    }
	    class doThFilterImpl_BUFFERFORMAT_BYTE1D_RGB_24 : IdoThFilterImpl
	    {
		    public void doThFilter(INyARRaster i_input, INyARRaster i_output,NyARIntSize i_size,int i_threshold)
		    {
                Debug.Assert(i_output.isEqualBufferType(NyARBufferType.INT1D_BIN_8));
    			
			    int[] out_buf = (int[]) i_output.getBuffer();
			    byte[] in_buf = (byte[]) i_input.getBuffer();
    			
			    int th=i_threshold*3;
			    int bp =(i_size.w*i_size.h-1)*3;
			    int w;
			    int xy;
			    int pix_count   =i_size.h*i_size.w;
			    int pix_mod_part=pix_count-(pix_count%8);
			    for(xy=pix_count-1;xy>=pix_mod_part;xy--){
				    w= ((in_buf[bp] & 0xff) + (in_buf[bp + 1] & 0xff) + (in_buf[bp + 2] & 0xff));
				    out_buf[xy]=w<=th?0:1;
				    bp -= 3;
			    }
			    //タイリング
			    for (;xy>=0;) {
				    w= ((in_buf[bp] & 0xff) + (in_buf[bp + 1] & 0xff) + (in_buf[bp + 2] & 0xff));
				    out_buf[xy]=w<=th?0:1;
				    bp -= 3;
				    xy--;
				    w= ((in_buf[bp] & 0xff) + (in_buf[bp + 1] & 0xff) + (in_buf[bp + 2] & 0xff));
				    out_buf[xy]=w<=th?0:1;
				    bp -= 3;
				    xy--;
				    w= ((in_buf[bp] & 0xff) + (in_buf[bp + 1] & 0xff) + (in_buf[bp + 2] & 0xff));
				    out_buf[xy]=w<=th?0:1;
				    bp -= 3;
				    xy--;
				    w= ((in_buf[bp] & 0xff) + (in_buf[bp + 1] & 0xff) + (in_buf[bp + 2] & 0xff));
				    out_buf[xy]=w<=th?0:1;
				    bp -= 3;
				    xy--;
				    w= ((in_buf[bp] & 0xff) + (in_buf[bp + 1] & 0xff) + (in_buf[bp + 2] & 0xff));
				    out_buf[xy]=w<=th?0:1;
				    bp -= 3;
				    xy--;
				    w= ((in_buf[bp] & 0xff) + (in_buf[bp + 1] & 0xff) + (in_buf[bp + 2] & 0xff));
				    out_buf[xy]=w<=th?0:1;
				    bp -= 3;
				    xy--;
				    w= ((in_buf[bp] & 0xff) + (in_buf[bp + 1] & 0xff) + (in_buf[bp + 2] & 0xff));
				    out_buf[xy]=w<=th?0:1;
				    bp -= 3;
				    xy--;
				    w= ((in_buf[bp] & 0xff) + (in_buf[bp + 1] & 0xff) + (in_buf[bp + 2] & 0xff));
				    out_buf[xy]=w<=th?0:1;
				    bp -= 3;
				    xy--;
			    }
			    return;			
		    }
    		
	    }
	    class doThFilterImpl_BUFFERFORMAT_BYTE1D_B8G8R8X8_32 : IdoThFilterImpl
	    {
		    public void doThFilter(INyARRaster i_input, INyARRaster i_output,NyARIntSize i_size,int i_threshold)
		    {
                Debug.Assert(i_input.isEqualBufferType(NyARBufferType.BYTE1D_B8G8R8X8_32));
                Debug.Assert(i_output.isEqualBufferType(NyARBufferType.INT1D_BIN_8));
    			
			    int[] out_buf = (int[]) i_output.getBuffer();
			    byte[] in_buf = (byte[]) i_input.getBuffer();
    			
			    int th=i_threshold*3;
			    int bp =(i_size.w*i_size.h-1)*4;
			    int w;
			    int xy;
			    int pix_count   =i_size.h*i_size.w;
			    int pix_mod_part=pix_count-(pix_count%8);
			    for(xy=pix_count-1;xy>=pix_mod_part;xy--){
				    w= ((in_buf[bp] & 0xff) + (in_buf[bp + 1] & 0xff) + (in_buf[bp + 2] & 0xff));
				    out_buf[xy]=w<=th?0:1;
				    bp -= 4;
			    }
			    //タイリング
			    for (;xy>=0;) {
				    w= ((in_buf[bp] & 0xff) + (in_buf[bp + 1] & 0xff) + (in_buf[bp + 2] & 0xff));
				    out_buf[xy]=w<=th?0:1;
				    bp -= 4;
				    xy--;
				    w= ((in_buf[bp] & 0xff) + (in_buf[bp + 1] & 0xff) + (in_buf[bp + 2] & 0xff));
				    out_buf[xy]=w<=th?0:1;
				    bp -= 4;
				    xy--;
				    w= ((in_buf[bp] & 0xff) + (in_buf[bp + 1] & 0xff) + (in_buf[bp + 2] & 0xff));
				    out_buf[xy]=w<=th?0:1;
				    bp -= 4;
				    xy--;
				    w= ((in_buf[bp] & 0xff) + (in_buf[bp + 1] & 0xff) + (in_buf[bp + 2] & 0xff));
				    out_buf[xy]=w<=th?0:1;
				    bp -= 4;
				    xy--;
				    w= ((in_buf[bp] & 0xff) + (in_buf[bp + 1] & 0xff) + (in_buf[bp + 2] & 0xff));
				    out_buf[xy]=w<=th?0:1;
				    bp -= 4;
				    xy--;
				    w= ((in_buf[bp] & 0xff) + (in_buf[bp + 1] & 0xff) + (in_buf[bp + 2] & 0xff));
				    out_buf[xy]=w<=th?0:1;
				    bp -= 4;
				    xy--;
				    w= ((in_buf[bp] & 0xff) + (in_buf[bp + 1] & 0xff) + (in_buf[bp + 2] & 0xff));
				    out_buf[xy]=w<=th?0:1;
				    bp -= 4;
				    xy--;
				    w= ((in_buf[bp] & 0xff) + (in_buf[bp + 1] & 0xff) + (in_buf[bp + 2] & 0xff));
				    out_buf[xy]=w<=th?0:1;
				    bp -= 4;
				    xy--;
			    }			
		    }		
	    }
    	
	    class doThFilterImpl_BUFFERFORMAT_BYTE1D_X8R8G8B8_32 : IdoThFilterImpl
	    {
		    public void doThFilter(INyARRaster i_input, INyARRaster i_output,NyARIntSize i_size,int i_threshold)
		    {
                Debug.Assert(i_output.isEqualBufferType(NyARBufferType.INT1D_BIN_8));
    			
			    int[] out_buf = (int[]) i_output.getBuffer();
			    byte[] in_buf = (byte[]) i_input.getBuffer();
    			
			    int th=i_threshold*3;
			    int bp =(i_size.w*i_size.h-1)*4;
			    int w;
			    int xy;
			    int pix_count   =i_size.h*i_size.w;
			    int pix_mod_part=pix_count-(pix_count%8);
			    for(xy=pix_count-1;xy>=pix_mod_part;xy--){
				    w= ((in_buf[bp+1] & 0xff) + (in_buf[bp + 2] & 0xff) + (in_buf[bp + 3] & 0xff));
				    out_buf[xy]=w<=th?0:1;
				    bp -= 4;
			    }
			    //タイリング
			    for (;xy>=0;) {
				    w= ((in_buf[bp+1] & 0xff) + (in_buf[bp + 2] & 0xff) + (in_buf[bp + 3] & 0xff));
				    out_buf[xy]=w<=th?0:1;
				    bp -= 4;
				    xy--;
				    w= ((in_buf[bp+1] & 0xff) + (in_buf[bp + 2] & 0xff) + (in_buf[bp + 3] & 0xff));
				    out_buf[xy]=w<=th?0:1;
				    bp -= 4;
				    xy--;
				    w= ((in_buf[bp+1] & 0xff) + (in_buf[bp + 2] & 0xff) + (in_buf[bp + 3] & 0xff));
				    out_buf[xy]=w<=th?0:1;
				    bp -= 4;
				    xy--;
				    w= ((in_buf[bp+1] & 0xff) + (in_buf[bp + 2] & 0xff) + (in_buf[bp + 3] & 0xff));
				    out_buf[xy]=w<=th?0:1;
				    bp -= 4;
				    xy--;
				    w= ((in_buf[bp+1] & 0xff) + (in_buf[bp + 2] & 0xff) + (in_buf[bp + 3] & 0xff));
				    out_buf[xy]=w<=th?0:1;
				    bp -= 4;
				    xy--;
				    w= ((in_buf[bp+1] & 0xff) + (in_buf[bp + 2] & 0xff) + (in_buf[bp + 3] & 0xff));
				    out_buf[xy]=w<=th?0:1;
				    bp -= 4;
				    xy--;
				    w= ((in_buf[bp+1] & 0xff) + (in_buf[bp + 2] & 0xff) + (in_buf[bp + 3] & 0xff));
				    out_buf[xy]=w<=th?0:1;
				    bp -= 4;
				    xy--;
				    w= ((in_buf[bp+1] & 0xff) + (in_buf[bp + 2] & 0xff) + (in_buf[bp + 3] & 0xff));
				    out_buf[xy]=w<=th?0:1;
				    bp -= 4;
				    xy--;
			    }
			    return;			
		    }
    		
	    }	
    	
	    class doThFilterImpl_BUFFERFORMAT_INT1D_X8R8G8B8_32 : IdoThFilterImpl
	    {
		    public void doThFilter(INyARRaster i_input, INyARRaster i_output,NyARIntSize i_size,int i_threshold)
		    {
                Debug.Assert(i_output.isEqualBufferType(NyARBufferType.INT1D_BIN_8));
    			
			    int[] out_buf = (int[]) i_output.getBuffer();
			    int[] in_buf = (int[]) i_input.getBuffer();
    			
			    int th=i_threshold*3;
			    int w;
			    int xy;
			    int pix_count   =i_size.h*i_size.w;
			    int pix_mod_part=pix_count-(pix_count%8);

			    for(xy=pix_count-1;xy>=pix_mod_part;xy--){
				    w=in_buf[xy];
				    out_buf[xy]=(((w>>16)&0xff)+((w>>8)&0xff)+(w&0xff))<=th?0:1;
			    }
			    //タイリング
			    for (;xy>=0;) {
				    w=in_buf[xy];
				    out_buf[xy]=(((w>>16)&0xff)+((w>>8)&0xff)+(w&0xff))<=th?0:1;
				    xy--;
				    w=in_buf[xy];
				    out_buf[xy]=(((w>>16)&0xff)+((w>>8)&0xff)+(w&0xff))<=th?0:1;
				    xy--;
				    w=in_buf[xy];
				    out_buf[xy]=(((w>>16)&0xff)+((w>>8)&0xff)+(w&0xff))<=th?0:1;
				    xy--;
				    w=in_buf[xy];
				    out_buf[xy]=(((w>>16)&0xff)+((w>>8)&0xff)+(w&0xff))<=th?0:1;
				    xy--;
				    w=in_buf[xy];
				    out_buf[xy]=(((w>>16)&0xff)+((w>>8)&0xff)+(w&0xff))<=th?0:1;
				    xy--;
				    w=in_buf[xy];
				    out_buf[xy]=(((w>>16)&0xff)+((w>>8)&0xff)+(w&0xff))<=th?0:1;
				    xy--;
				    w=in_buf[xy];
				    out_buf[xy]=(((w>>16)&0xff)+((w>>8)&0xff)+(w&0xff))<=th?0:1;
				    xy--;
				    w=in_buf[xy];
				    out_buf[xy]=(((w>>16)&0xff)+((w>>8)&0xff)+(w&0xff))<=th?0:1;
				    xy--;
			    }			
		    }		
	    }
    	
	    class doThFilterImpl_BUFFERFORMAT_WORD1D_R5G6B5_16LE : IdoThFilterImpl
	    {
		    public void doThFilter(INyARRaster i_input, INyARRaster i_output,NyARIntSize i_size,int i_threshold)
		    {
                Debug.Assert(i_output.isEqualBufferType(NyARBufferType.INT1D_BIN_8));
    			
			    int[] out_buf = (int[]) i_output.getBuffer();
			    short[] in_buf = (short[]) i_input.getBuffer();
    			
			    int th=i_threshold*3;
			    int w;
			    int xy;
			    int pix_count   =i_size.h*i_size.w;
			    int pix_mod_part=pix_count-(pix_count%8);

			    for(xy=pix_count-1;xy>=pix_mod_part;xy--){				
                    w =(int)in_buf[xy];
                    w = ((w & 0xf800) >> 8) + ((w & 0x07e0) >> 3) + ((w & 0x001f) << 3);
                    out_buf[xy] = w <= th ? 0 : 1;
			    }
			    //タイリング
			    for (;xy>=0;) {
                    w =(int)in_buf[xy];
                    w = ((w & 0xf800) >> 8) + ((w & 0x07e0) >> 3) + ((w & 0x001f) << 3);
                    out_buf[xy] = w <= th ? 0 : 1;
				    xy--;
                    w =(int)in_buf[xy];
                    w = ((w & 0xf800) >> 8) + ((w & 0x07e0) >> 3) + ((w & 0x001f) << 3);
                    out_buf[xy] = w <= th ? 0 : 1;
				    xy--;
                    w =(int)in_buf[xy];
                    w = ((w & 0xf800) >> 8) + ((w & 0x07e0) >> 3) + ((w & 0x001f) << 3);
                    out_buf[xy] = w <= th ? 0 : 1;
				    xy--;
                    w =(int)in_buf[xy];
                    w = ((w & 0xf800) >> 8) + ((w & 0x07e0) >> 3) + ((w & 0x001f) << 3);
                    out_buf[xy] = w <= th ? 0 : 1;
				    xy--;
                    w =(int)in_buf[xy];
                    w = ((w & 0xf800) >> 8) + ((w & 0x07e0) >> 3) + ((w & 0x001f) << 3);
                    out_buf[xy] = w <= th ? 0 : 1;
				    xy--;
                    w =(int)in_buf[xy];
                    w = ((w & 0xf800) >> 8) + ((w & 0x07e0) >> 3) + ((w & 0x001f) << 3);
                    out_buf[xy] = w <= th ? 0 : 1;
				    xy--;
                    w =(int)in_buf[xy];
                    w = ((w & 0xf800) >> 8) + ((w & 0x07e0) >> 3) + ((w & 0x001f) << 3);
                    out_buf[xy] = w <= th ? 0 : 1;
				    xy--;
                    w =(int)in_buf[xy];
                    w = ((w & 0xf800) >> 8) + ((w & 0x07e0) >> 3) + ((w & 0x001f) << 3);
                    out_buf[xy] = w <= th ? 0 : 1;
				    xy--;
			    }
		    }		
	    }	
    }
}
