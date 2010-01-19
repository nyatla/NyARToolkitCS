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
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace jp.nyatla.nyartoolkit.cs.core
{
    public class NyARRasterFilter_ConstantThreshold : INyARRasterFilter_Gs2Bin
    {
	    public int _threshold;
	    public NyARRasterFilter_ConstantThreshold(int i_initial_threshold,int i_in_raster_type,int i_out_raster_type)
	    {
		    Debug.Assert(i_in_raster_type==NyARBufferType.INT1D_GRAY_8);
		    Debug.Assert(i_out_raster_type==NyARBufferType.INT1D_BIN_8);
		    //初期化
		    this._threshold=i_initial_threshold;
    		
	    }
	    /**
	     * ２値化の閾値を設定する。
	     * 暗点<=th<明点となります。
	     * @throws NyARException
	     */
	    public NyARRasterFilter_ConstantThreshold()
	    {
		    this._threshold=0;
	    }

    	
	    public void setThreshold(int i_threshold)
	    {
		    this._threshold = i_threshold;
	    }
	    public void doFilter(NyARGrayscaleRaster i_input, NyARBinRaster i_output)
	    {
		    Debug.Assert(i_input.getBufferType()==NyARBufferType.INT1D_GRAY_8);
		    Debug.Assert(i_output.getBufferType()==NyARBufferType.INT1D_BIN_8);
		    int[] out_buf = (int[]) i_output.getBuffer();
		    int[] in_buf = (int[]) i_input.getBuffer();
		    NyARIntSize s=i_input.getSize();
    		
		    int th=this._threshold;
		    int bp =s.w*s.h-1;
		    int pix_count   =s.h*s.w;
		    int pix_mod_part=pix_count-(pix_count%8);
		    for(bp=pix_count-1;bp>=pix_mod_part;bp--){
			    out_buf[bp]=(in_buf[bp] & 0xff)<=th?0:1;
		    }
		    //タイリング
		    for (;bp>=0;) {
			    out_buf[bp]=(in_buf[bp] & 0xff)<=th?0:1;
			    bp--;
			    out_buf[bp]=(in_buf[bp] & 0xff)<=th?0:1;
			    bp--;
			    out_buf[bp]=(in_buf[bp] & 0xff)<=th?0:1;
			    bp--;
			    out_buf[bp]=(in_buf[bp] & 0xff)<=th?0:1;
			    bp--;
			    out_buf[bp]=(in_buf[bp] & 0xff)<=th?0:1;
			    bp--;
			    out_buf[bp]=(in_buf[bp] & 0xff)<=th?0:1;
			    bp--;
			    out_buf[bp]=(in_buf[bp] & 0xff)<=th?0:1;
			    bp--;
			    out_buf[bp]=(in_buf[bp] & 0xff)<=th?0:1;
			    bp--;
		    }
		    return;			
	    }
    }
}
