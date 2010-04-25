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

namespace jp.nyatla.nyartoolkit.cs.core
{
    public sealed class NyARHsvRaster : NyARRaster_BasicClass
    {

	    private int[] _ref_buf;
    	
	    public NyARHsvRaster(int i_width, int i_height)
            :base(i_width,i_height,NyARBufferType.INT1D_X7H9S8V8_32)
	    {
		    //このクラスは外部参照バッファ/形式多重化が使えない簡易実装です。
		   
		    this._ref_buf = new int[i_height*i_width];
	    }
	    public override object getBuffer()
	    {
		    return this._ref_buf;
	    }
        public override bool hasBuffer()
	    {
		    return true;
	    }
        public override void wrapBuffer(Object i_ref_buf)
	    {
		    NyARException.notImplement();
	    }	
    }
}
